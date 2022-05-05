using MassTransit;
using TripBooker.Common.Order.Transport;
using TripBooker.TransportService.Services;

namespace TripBooker.TransportService.EventConsumers.Public;

internal class CancelReservationEventConsumer : IConsumer<CancelTransportReservation>
{
    private readonly ITransportReservationService _reservationService;
    private readonly ILogger<CancelReservationEventConsumer> _logger;

    public CancelReservationEventConsumer(
        ITransportReservationService reservationService, 
        ILogger<CancelReservationEventConsumer> logger)
    {
        _reservationService = reservationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CancelTransportReservation> context)
    {
        _logger.LogInformation(
            $"Received cancel transport reservation for order (OrderId={context.Message.CorrelationId}, " +
            $"ReservationId={context.Message.ReservationId}).");

        await _reservationService.Cancel(context.Message.ReservationId, context.CancellationToken);

        _logger.LogInformation(
            $"Consumed cancel transport reservation for order (OrderId={context.Message.CorrelationId}, " +
            $"ReservationId={context.Message.ReservationId}).");

    }
}