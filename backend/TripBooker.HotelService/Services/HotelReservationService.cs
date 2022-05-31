using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Transactions;
using TripBooker.Common;
using TripBooker.Common.Hotel;
using TripBooker.Common.Order;
using TripBooker.Common.Order.Hotel;
using TripBooker.HotelService.Model;
using TripBooker.HotelService.Model.Events;
using TripBooker.HotelService.Model.Events.Hotel;
using TripBooker.HotelService.Model.Events.Reservation;
using TripBooker.HotelService.Model.Extensions;
using TripBooker.HotelService.Repositories;

namespace TripBooker.HotelService.Services;

internal interface IHotelReservationService
{
    Task<ReservationModel> AddNewReservation(NewHotelReservation newReservationContract, CancellationToken cancellationToken);

    Task Cancel(Guid reservationId, CancellationToken cancellationToken);

    Task Confirm(Guid reservationId, CancellationToken cancellationToken);
}

internal class HotelReservationService : IHotelReservationService
{
    private readonly IReservationEventRepository _reservationRepository;
    private readonly IHotelEventRepository _eventRepository;
    private readonly IHotelOptionRepository _hotelRepository;
    private readonly ILogger<HotelReservationService> _logger;

    public HotelReservationService(
        IReservationEventRepository reservationRepository, 
        IHotelEventRepository eventRepository, 
        IHotelOptionRepository hotelRepository, 
        ILogger<HotelReservationService> logger)
    {
        _reservationRepository = reservationRepository;
        _eventRepository = eventRepository;
        _hotelRepository = hotelRepository;
        _logger = logger;
    }

    public async Task<ReservationModel> AddNewReservation(NewHotelReservation reservation, CancellationToken cancellationToken)
    {
        var order = reservation.Order;

        var data = new NewReservationEventData(order.HotelDays,
            order.RoomsStudio,
            order.RoomsSmall,
            order.RoomsMedium,
            order.RoomsLarge,
            order.RoomsApartment,
            order.MealOption);
        var reservationStreamId = await _reservationRepository.AddNewAsync(data, cancellationToken);
        var destinationAirportCode = string.Empty;
        
        bool transactionSuccesfull;
        do
        {
            transactionSuccesfull = true;

            var hotelOccupations = new List<HotelOccupationModel>();

            // Check hotel
            var hotel = await _hotelRepository.QueryAll()
                .FirstOrDefaultAsync(x => x.Code == order.HotelCode, cancellationToken: cancellationToken);
            if (hotel == null || (hotel.AllInclusive == false && order.MealOption == MealOption.AllInclusive))
            {
                // There is no such hotel or the hotel cannot provide required service
                await _reservationRepository.AddRejectedAsync(reservationStreamId, 1, cancellationToken);
                break;
            }
            
            destinationAirportCode = hotel.AirportCode;

            // Check for all days
            var checkSuccessful = true;
            foreach(var hotelDay in order.HotelDays)
            {
                var hotelEvents = await _eventRepository.GetHotelEventsAsync(hotelDay, cancellationToken);
                var occupation = HotelOccupationBuilder.Build(hotelEvents);

                // Check if enough rooms awailable
                if (occupation.RoomsStudio < order.RoomsStudio 
                    || occupation.RoomsSmall < order.RoomsSmall 
                    || occupation.RoomsMedium < order.RoomsMedium 
                    || occupation.RoomsLarge < order.RoomsLarge 
                    || occupation.RoomsApartment < order.RoomsApartment)
                {
                    checkSuccessful = false;
                    break;
                }

                hotelOccupations.Add(occupation);
            }

            if (!checkSuccessful)
            {
                // There is not enough free rooms
                await _reservationRepository.AddRejectedAsync(reservationStreamId, 1, cancellationToken);
                break;
            }

            // Check succesfull can reserve
            try
            {
                await ValidateNewReservationTransaction(reservationStreamId, order, hotel, hotelOccupations,
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
        var result = ReservationBuilder.Build(reservationEvents);
        result.DestinationAirportCode = destinationAirportCode;

        return result;
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

    public async Task Confirm(Guid reservationId, CancellationToken cancellationToken)
    {
        var tryTransaction = true;
        while (tryTransaction)
        {
            tryTransaction = false;

            var reservationEvents =
                await _reservationRepository.GetReservationEvents(reservationId, cancellationToken);
            var reservation = ReservationBuilder.Build(reservationEvents);

            if (reservation.Status == ReservationStatus.Rejected)
                _logger.LogWarning($"Cannot confirm rejected reservation (ReservationId={reservation})");

            if (reservation.Status != ReservationStatus.Accepted)
                return;

            try
            {
                await _reservationRepository.AddConfirmedAsync(reservation.Id, reservation.Version,
                    cancellationToken);
            }
            catch (DbUpdateException e)
            {
                if (e.GetBaseException() is PostgresException { SqlState: GlobalConstants.PostgresUniqueViolationCode })
                {
                    // repeat if there was version violation, so the db read and business logic
                    // does not need to be inside transaction
                    tryTransaction = true;
                }
                else
                {
                    throw;
                }
            }
        }
    }

    private async Task ValidateNewReservationTransaction(Guid reservationStreamId,
                                                         OrderData order,
                                                         HotelOption hotel,
                                                         List<HotelOccupationModel> hotelOccupations,
                                                         CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var updateEvent = new OccupatonUpdateEvent
        {
            ReservationEventId = reservationStreamId,
            RoomsApartment = -order.RoomsApartment,
            RoomsMedium = -order.RoomsMedium,
            RoomsLarge = -order.RoomsLarge,
            RoomsSmall = -order.RoomsSmall,
            RoomsStudio = -order.RoomsStudio
        };

        await _eventRepository.AddToManyAsync(updateEvent, hotelOccupations.Select(x => x.Id), 
            hotelOccupations.Select(x => x.Version), cancellationToken);

        var price = HotelExtensions.CalculatePrice(order, hotelOccupations, hotel);
        
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
