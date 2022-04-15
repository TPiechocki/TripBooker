using MassTransit;
using TripBooker.Common.Order.Transport;
using TripBooker.TransportService.Services;

namespace TripBooker.TransportService.EventConsumers.Public;

internal class CancelReservationEventConsumer : IConsumer<CancelTransportReservation>
{
    private readonly ITransportReservationService _reservationService;

    public CancelReservationEventConsumer(ITransportReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public async Task Consume(ConsumeContext<CancelTransportReservation> context)
    {
        await _reservationService.Cancel(context.Message.ReservationId, context.CancellationToken);
    }
}