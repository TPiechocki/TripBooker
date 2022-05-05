using MassTransit;
using TripBooker.Common.Order.Hotel;
using TripBooker.HotelService.Services;

namespace TripBooker.HotelService.EventConsumers.Public;

internal class CancelReservationEventConsumer : IConsumer<CancelHotelReservation>
{
    private readonly IHotelReservationService _reservationService;
    private readonly ILogger<CancelReservationEventConsumer> _logger;

    public CancelReservationEventConsumer(
        IHotelReservationService reservationService, 
        ILogger<CancelReservationEventConsumer> logger)
    {
        _reservationService = reservationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CancelHotelReservation> context)
    {
        _logger.LogInformation(
            $"Received cancel hotel reservation for order (OrderId={context.Message.CorrelationId}, " +
            $"ReservationId={context.Message.ReservationId}).");

        await _reservationService.Cancel(context.Message.ReservationId, context.CancellationToken);

        _logger.LogInformation(
            $"Consumed cancel hotel reservation for order (OrderId={context.Message.CorrelationId}, " +
            $"ReservationId={context.Message.ReservationId}).");
    }
}
