using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Transactions;
using TripBooker.Common;
using TripBooker.Common.Hotel;
using TripBooker.Common.Order.Hotel;
using TripBooker.HotelService.Model;
using TripBooker.HotelService.Model.Events;
using TripBooker.HotelService.Model.Events.Hotel;
using TripBooker.HotelService.Repositories;

namespace TripBooker.HotelService.Services;

internal interface IHotelReservationService
{
    Task<ReservationModel> AddNewReservation(NewHotelReservation newReservationContract, CancellationToken cancellationToken);

    Task Cancel(Guid reservationId, CancellationToken cancellationToken);
}

internal class HotelReservationService : IHotelReservationService
{
    private readonly IReservationEventRepository _reservationRepository;
    private readonly IHotelEventRepository _eventRepository;
    private readonly IHotelOptionRepository _hotelRepository;

    public HotelReservationService(IReservationEventRepository reservationRepository, IHotelEventRepository eventRepository, IHotelOptionRepository hotelRepository)
    {
        _reservationRepository = reservationRepository;
        _eventRepository = eventRepository;
        _hotelRepository = hotelRepository;
    }

    public async Task<ReservationModel> AddNewReservation(NewHotelReservation reservation, CancellationToken cancellationToken)
    {
        var data = new NewReservationEventData(reservation.HotelDays,
                                               reservation.RoomsStudio,
                                               reservation.RoomsSmall,
                                               reservation.RoomsMedium,
                                               reservation.RoomsLarge,
                                               reservation.RoomsApartment,
                                               reservation.MealOption);
        var reservationStreamId = await _reservationRepository.AddNewAsync(data, cancellationToken);

        bool transactionSuccesfull;
        do
        {
            transactionSuccesfull = true;

            var hotelOccupations = new List<HotelOccupationModel>();

            // Check hotel
            var hotel = await _hotelRepository.GetByIdAsync(reservation.Order.HotelId, cancellationToken);
            if (hotel == null || (hotel.AllInclusive == false && reservation.MealOption == MealOption.AllInclusive))
            {
                // There is no such hotel or the hotel cannot provide required service
                await _reservationRepository.AddRejectedAsync(reservationStreamId, 1, cancellationToken);
                break;
            }

            // Check for all days
            bool checkSuccesfull = true;
            foreach(var hotelday in reservation.HotelDays)
            {
                var hotelEvents = await _eventRepository.GetHotelEventsAsync(hotelday, cancellationToken);
                var occupation = HotelOccupationBuilder.Build(hotelEvents);

                // Check if enough rooms awailable
                if (occupation.RoomsStudio < reservation.RoomsStudio 
                    && occupation.RoomsSmall < reservation.RoomsSmall 
                    && occupation.RoomsMedium < reservation.RoomsMedium 
                    && occupation.RoomsLarge < reservation.RoomsLarge 
                    && occupation.RoomsApartment < reservation.RoomsApartment)
                {
                    checkSuccesfull = false;
                    break;
                }

                hotelOccupations.Add(occupation);
            }

            if (!checkSuccesfull)
            {
                // There is not enough free rooms
                await _reservationRepository.AddRejectedAsync(reservationStreamId, 1, cancellationToken);
                break;
            }

            // Check succesfull can reserve
            try
            {
                await ValidateNewReservationTransaction(reservationStreamId, reservation, hotel, hotelOccupations,
                    cancellationToken);
            }
            catch (DbUpdateException e)
            {
                if (e.GetBaseException() is PostgresException { SqlState: GlobalConstants.PostgresUniqueViolationCode })
                {
                    // repeat if there was version violation, so the db read and business logic
                    // does not need to be inside transaction
                    transactionSuccesfull = false;
                }
                else
                {
                    throw;
                }
            }

        } 
        while (!transactionSuccesfull);

        var reservationEvents =
            await _reservationRepository.GetReservationEvents(reservationStreamId, cancellationToken);
        return ReservationBuilder.Build(reservationEvents);
    }

    public async Task Cancel(Guid reservationId, CancellationToken cancellationToken)
    {
        var reservationEvents =
            await _reservationRepository.GetReservationEvents(reservationId, cancellationToken);
        var reservation = ReservationBuilder.Build(reservationEvents);

        if (reservation.Status != ReservationStatus.Accepted && reservation.Status != ReservationStatus.Confirmed)
            return;

        bool transactionSuccesfull;
        do
        {
            transactionSuccesfull = true;

            var hotelOccupations = new List<HotelOccupationModel>();

            foreach (var hotelday in reservation.HotelDays)
            {
                var hotelEvents = await _eventRepository.GetHotelEventsAsync(hotelday, cancellationToken);
                var occupation = HotelOccupationBuilder.Build(hotelEvents);

                hotelOccupations.Add(occupation);
            }

            try
            {
                await ValidateCancelReservationTransaction(reservation, hotelOccupations, cancellationToken);
            }
            catch (DbUpdateException e)
            {
                if (e.GetBaseException() is PostgresException { SqlState: GlobalConstants.PostgresUniqueViolationCode })
                {
                    // repeat if there was version violation, so the db read and business logic
                    // does not need to be inside transaction
                    transactionSuccesfull = false;
                }
                else
                {
                    throw;
                }
            }

        }
        while (!transactionSuccesfull);
    }

    private async Task ValidateNewReservationTransaction(Guid reservationStreamId,
                                                         NewHotelReservation reservation,
                                                         HotelOption hotel,
                                                         List<HotelOccupationModel> hotelOccupations,
                                                         CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var updateEvent = new OccupatonUpdateEvent
        {
            ReservationEventId = reservationStreamId,
            RoomsApartment = -reservation.RoomsApartment,
            RoomsMedium = -reservation.RoomsMedium,
            RoomsLarge = -reservation.RoomsLarge,
            RoomsSmall = -reservation.RoomsSmall,
            RoomsStudio = -reservation.RoomsStudio
        };

        await _eventRepository.AddToManyAsync(updateEvent, hotelOccupations.Select(x => x.Id), 
            hotelOccupations.Select(x => x.Version), cancellationToken);

        // Calculate price
        var price = reservation.RoomsStudio * hotel.GetPriceFor(RoomType.Studio)
                    + reservation.RoomsSmall * hotel.GetPriceFor(RoomType.Small)
                    + reservation.RoomsMedium * hotel.GetPriceFor(RoomType.Medium)
                    + reservation.RoomsLarge * hotel.GetPriceFor(RoomType.Large)
                    + reservation.RoomsApartment * hotel.GetPriceFor(RoomType.Apartment)
                    + reservation.Order.NumberOfOccupiedSeats * hotel.GetPriceFor(reservation.MealOption);

        await _reservationRepository.AddAcceptedAsync(reservationStreamId, 1, new ReservationAcceptedEventData(price), cancellationToken);

        transaction.Complete();
    }

    private async Task ValidateCancelReservationTransaction(ReservationModel reservation, 
        IEnumerable<HotelOccupationModel> hotelOccupations, CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var updateEvent = new OccupatonUpdateEvent
        {
            ReservationEventId = reservation.Id,
            RoomsApartment = reservation.RoomsApartment,
            RoomsMedium = reservation.RoomsMedium,
            RoomsLarge = reservation.RoomsLarge,
            RoomsSmall = reservation.RoomsSmall,
            RoomsStudio = reservation.RoomsStudio
        };

        await _eventRepository.AddToManyAsync(updateEvent, hotelOccupations.Select(x => x.Id),
            hotelOccupations.Select(x => x.Version), cancellationToken);

        await _reservationRepository.AddCancelledAsync(reservation.Id, reservation.Version, cancellationToken);

        transaction.Complete();
    }
}
