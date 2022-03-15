using System.Transactions;
using TripBooker.Common.Transport;
using TripBooker.TransportService.Model;
using TripBooker.TransportService.Repositories;

namespace TripBooker.TransportService.Services;

internal interface ITransportReservationService
{
    Task AddNewReservation(TransportReservation reservation, CancellationToken cancellationToken);
}

internal class TransportReservationService : ITransportReservationService
{
    private readonly ITransportViewUpdateRepository _viewRepository;
    private readonly ITransportReservationRepository _reservationRepository;


    public TransportReservationService(ITransportViewUpdateRepository viewRepository, 
        ITransportReservationRepository reservationRepository)
    {
        _viewRepository = viewRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task AddNewReservation(TransportReservation reservation, CancellationToken cancellationToken)
    {
        // TODO: replace with event sourcing

        // check if places
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        // remove places from the transport pool
        var view = await _viewRepository.QueryByIdAsync(reservation.TranportId, cancellationToken);
        if (view == null || view.AvailablePlaces < reservation.Places)
        {
            reservation.Status = ReservationStatus.Rejected;
        }
        else
        {
            reservation.Status = ReservationStatus.AwaitingPayment;
            view.AvailablePlaces -= reservation.Places;
            _viewRepository.Update(view);
        }

        // add reservation
        await _reservationRepository.AddAsync(reservation, cancellationToken);

        // TODO: add event

        transaction.Complete();

        // TODO: response with status
    }
}