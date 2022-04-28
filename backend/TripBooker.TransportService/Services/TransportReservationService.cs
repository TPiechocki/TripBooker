using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Transactions;
using Newtonsoft.Json;
using TripBooker.Common;
using TripBooker.Common.Extensions;
using TripBooker.Common.Order.Transport;
using TripBooker.TransportService.Model;
using TripBooker.TransportService.Model.Events;
using TripBooker.TransportService.Model.Events.Reservation;
using TripBooker.TransportService.Model.Events.Transport;
using TripBooker.TransportService.Repositories;

namespace TripBooker.TransportService.Services;

internal interface ITransportReservationService
{
    Task<ReservationModel> AddNewReservation(NewTransportReservation reservation, CancellationToken cancellationToken);

    Task Cancel(Guid reservationId, CancellationToken cancellationToken);
}

internal class TransportReservationService : ITransportReservationService
{
    private readonly ITransportEventRepository _transportRepository;
    private readonly IReservationEventRepository _reservationEventRepository;

    public TransportReservationService(
        ITransportEventRepository transportRepository,
        IReservationEventRepository reservationEventRepository)
    {
        _transportRepository = transportRepository;
        _reservationEventRepository = reservationEventRepository;
    }

    public async Task<ReservationModel> AddNewReservation(NewTransportReservation reservation, CancellationToken cancellationToken)
    {
        var transportId = reservation.IsReturn
            ? reservation.Order.ReturnTransportId
            : reservation.Order.TransportId;
        var numberOfPlaces = reservation.Order.NumberOfOccupiedSeats();

        // add reservation
        var data = new NewReservationEventData(transportId, numberOfPlaces);
        var reservationStreamId = await _reservationEventRepository.AddNewAsync(data, cancellationToken);

        var tryTransaction = true;

        while (tryTransaction)
        {
            tryTransaction = false;

            var transportEvents =
                await _transportRepository.GetTransportEventsAsync(transportId, cancellationToken);
            if (transportEvents.Count == 0)
            {
                await _reservationEventRepository.AddRejectedAsync(reservationStreamId, 1, cancellationToken);
                throw new ArgumentException(
                    $"Received reservation for transport which does not exist {JsonConvert.SerializeObject(reservation)}.",
                    nameof(reservation));
            }

            var transportItem = TransportBuilder.Build(transportEvents);

            if (transportItem.AvailablePlaces < numberOfPlaces)
            {
                // if there is not enough free places
                await _reservationEventRepository.AddRejectedAsync(reservationStreamId, 1, cancellationToken);
                break;
            }

            try
            {
                await ValidateNewReservationTransaction(reservationStreamId, transportId, numberOfPlaces, 
                    transportItem, cancellationToken);
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
                    await _reservationEventRepository.AddRejectedAsync(reservationStreamId, 1, cancellationToken);
                    throw;
                }
            }
        }

        // read can be safely outside transaction as reservationId is not known by anybody else at this point
        var reservationEvents = 
            await _reservationEventRepository.GetReservationEvents(reservationStreamId, cancellationToken);
        return ReservationBuilder.Build(reservationEvents);
    }

    public async Task Cancel(Guid reservationId, CancellationToken cancellationToken) 
    {
        
        var reservationEvents =
            await _reservationEventRepository.GetReservationEvents(reservationId, cancellationToken);
        var reservation = ReservationBuilder.Build(reservationEvents);

        if (reservation.Status != ReservationStatus.Accepted && reservation.Status != ReservationStatus.Confirmed)
            return;

        var tryTransaction = true;
        while (tryTransaction)
        {
            tryTransaction = false;

            var transportEvents =
                await _transportRepository.GetTransportEventsAsync(reservation.TransportId, cancellationToken);

            var transportItem = TransportBuilder.Build(transportEvents);

            try
            {
                await ValidateCancelReservationTransaction(reservation, transportItem, cancellationToken);
            }
            catch (DbUpdateException e)
            {
                if (e.GetBaseException() is PostgresException {SqlState: GlobalConstants.PostgresUniqueViolationCode })
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

    private async Task ValidateNewReservationTransaction(Guid reservationStreamId, Guid transportId,
        int numberOfPlaces, TransportModel transportItem,
        CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var transportEvent = new TransportPlaceUpdateEvent(
            transportItem.AvailablePlaces - numberOfPlaces,
            -numberOfPlaces,
            reservationStreamId);

        await _transportRepository.AddAsync(transportEvent, transportId, transportItem.Version,
            cancellationToken);

        var price = numberOfPlaces * transportItem.TicketPrice;

        await _reservationEventRepository.AddAcceptedAsync(reservationStreamId, 1,
            new ReservationAcceptedEventData(price), cancellationToken);

        transaction.Complete();
    }

    private async Task ValidateCancelReservationTransaction(ReservationModel reservation,
        TransportModel transportItem,
        CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var transportEvent = new TransportPlaceUpdateEvent(
            transportItem.AvailablePlaces + reservation.Places,
            reservation.Places,
            reservation.Id);

        await _transportRepository.AddAsync(transportEvent, reservation.TransportId, transportItem.Version,
            cancellationToken);

        await _reservationEventRepository.AddCancelledAsync(reservation.Id, reservation.Version, cancellationToken);

        transaction.Complete();
    }
}