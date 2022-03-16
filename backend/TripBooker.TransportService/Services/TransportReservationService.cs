using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TripBooker.TransportService.Contract;
using TripBooker.TransportService.Model;
using TripBooker.TransportService.Model.Events;
using TripBooker.TransportService.Model.Events.Reservation;
using TripBooker.TransportService.Model.Events.Transport;
using TripBooker.TransportService.Repositories;

namespace TripBooker.TransportService.Services;

internal interface ITransportReservationService
{
    Task<Guid> AddNewReservation(NewReservationContract reservation, CancellationToken cancellationToken);
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

    public async Task<Guid> AddNewReservation(NewReservationContract reservation, CancellationToken cancellationToken)
    {
        // add reservation
        var data = new NewReservationEventData(reservation.TransportId, reservation.Places);
        var reservationStreamId = await _reservationEventRepository.AddNewAsync(data, cancellationToken);

        var tryTransaction = true;

        while (tryTransaction)
        {
            tryTransaction = false;

            var transportEvents =
                await _transportRepository.GetTransportEvents(reservation.TransportId, cancellationToken);
            var transportItem = TransportBuilder.Build(transportEvents);

            try
            {
                await ValidateNewReservationTransaction(reservationStreamId, reservation, transportItem,
                    cancellationToken);
            }
            catch (DbUpdateException e)
            {
                if (e.GetBaseException() is PostgresException { SqlState: "23505" })
                {
                    // repeat if there was version violation, so read must not be inside transaction
                    tryTransaction = true;
                }
                else
                {
                    throw;
                }
            }
        }

        // TODO: response with status

        return reservationStreamId;
    }

    private async Task ValidateNewReservationTransaction(Guid reservationStreamId, NewReservationContract reservation,
        TransportModel transportItem,
        CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var transportEvent = new TransportPlaceUpdateEvent(
            transportItem.AvailablePlaces - reservation.Places,
            -reservation.Places,
            reservationStreamId);

        await _transportRepository.AddAsync(transportEvent, reservation.TransportId, transportItem.Version,
            cancellationToken);

        // TODO: set accepted/rejected status

        transaction.Complete();
    }
}