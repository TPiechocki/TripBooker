using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Transactions;
using TripBooker.Common;
using TripBooker.HotelService.Contract;
using TripBooker.HotelService.Model;
using TripBooker.HotelService.Model.Events;
using TripBooker.HotelService.Model.Events.Hotel;
using TripBooker.HotelService.Repositories;

namespace TripBooker.HotelService.Services;

internal interface IHotelReservationService
{
    Task AddNewReservation(NewReservationContract newReservationContract, CancellationToken cancellationToken);
}

internal class HotelReservationService : IHotelReservationService
{
    private readonly IReservationEventRepository _reservationRepository;
    private readonly IHotelEventRepository _hotelRepository;

    public HotelReservationService(ReservationEventRepository reservationRepository, HotelEventRepository hotelRepository)
    {
        _reservationRepository = reservationRepository;
        _hotelRepository = hotelRepository;
    }

    public async Task AddNewReservation(NewReservationContract reservation, CancellationToken cancellationToken)
    {
        var data = new NewReservationEventData(reservation.HotelDays,
                                               reservation.RoomsStudio,
                                               reservation.RoomsSmall,
                                               reservation.RoomsMedium,
                                               reservation.RoomsLarge,
                                               reservation.RoomsApartment);
        var reservationStreamId = await _reservationRepository.AddNewAsync(data, cancellationToken);

        bool transactionSuccesfull;
        do
        {
            transactionSuccesfull = true;

            var hotelOccupations = new List<HotelOccupationModel>();

            // Check for all days
            bool checkSuccesfull = true;
            foreach(var hotelday in reservation.HotelDays)
            {
                var hotelEvents = await _hotelRepository.GetTransportEventsAsync(hotelday, cancellationToken);
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
                await ValidateNewReservationTransaction(reservationStreamId, reservation, hotelOccupations,
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

    }

    private async Task ValidateNewReservationTransaction(Guid reservationStreamId, NewReservationContract reservation, 
        List<HotelOccupationModel> hotelOccupations, CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        foreach(var occupation in hotelOccupations)
        {
            var updateEvent = new OccupatonUpdateEvent
            {
                ReservationEventId = reservationStreamId,
                RoomsApartment = reservation.RoomsApartment,
                RoomsMedium = reservation.RoomsMedium,
                RoomsLarge = reservation.RoomsLarge,
                RoomsSmall = reservation.RoomsSmall,
                RoomsStudio = reservation.RoomsStudio
            };

            await _hotelRepository.AddAsync(updateEvent, occupation.Id, occupation.Version, cancellationToken);

        }

        await _reservationRepository.AddAcceptedAsync(reservationStreamId, 1, cancellationToken);

        transaction.Complete();
    }
}
