using MassTransit;
using TripBooker.Common.Order.Transport;
using TripBooker.TransportService.Services;

namespace TripBooker.TransportService.EventConsumers.Public;

internal class ConfirmTransportReservationEventConsumer : IConsumer<ConfirmTransportReservation>
{
    private readonly ITransportReservationService _reservationService;
    private readonly ILogger<ConfirmTransportReservationEventConsumer> _logger;

    public ConfirmTransportReservationEventConsumer(
        ITransportReservationService reservationService, 
        ILogger<ConfirmTransportReservationEventConsumer> logger)
    {
        _reservationService = reservationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ConfirmTransportReservation> context)
    {
        _logger.LogInformation(
            $"Received confirm transport reservation for order (OrderId={context.Message.CorrelationId}, " +
            $"ReservationId={context.Message.ReservationId}).");

        await _reservationService.Confirm(context.Message.ReservationId, context.CancellationToken);

        _logger.LogInformation(
            $"Consumed confirm transport reservation for order (OrderId={context.Message.CorrelationId}, " +
            $"ReservationId={context.Message.ReservationId}).");
    }
}