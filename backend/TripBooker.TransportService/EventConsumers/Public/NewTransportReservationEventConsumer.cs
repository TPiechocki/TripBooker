using MassTransit;
using TripBooker.Common;
using TripBooker.Common.Order.Transport;
using TripBooker.TransportService.Model.Events;
using TripBooker.TransportService.Services;

namespace TripBooker.TransportService.EventConsumers.Public;

internal class NewTransportReservationEventConsumer : IConsumer<NewTransportReservation>
{
    private readonly ITransportReservationService _reservationService;
    private readonly IBus _bus;
    private readonly ILogger<NewTransportReservationEventConsumer> _logger;

    public NewTransportReservationEventConsumer(
        ITransportReservationService reservationService, 
        IBus bus, 
        ILogger<NewTransportReservationEventConsumer> logger)
    {
        _reservationService = reservationService;
        _bus = bus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NewTransportReservation> context)
    {
        _logger.LogInformation($"New reservation received (OrderId={context.Message.Order.OrderId})");
        try
        {
            var result =
                await _reservationService.AddNewReservation(context.Message, context.CancellationToken);

            if (result.Status == ReservationStatus.Accepted)
            {
                await context.Publish(
                    new TransportReservationAccepted(context.Message.Order.OrderId, result.Price, result.Id),
                    context.CancellationToken);
                await _bus.Publish(new TransportViewUpdateEvent(), context.CancellationToken);
                _logger.LogInformation($"Reservation accepted (OrderId={context.Message.Order.OrderId})");
            }
            else
            {
                await context.Publish(new TransportReservationRejected(context.Message.Order.OrderId, result.Id), context.CancellationToken);
                _logger.LogInformation($"Reservation rejected (OrderId={context.Message.Order.OrderId})");
            }
        }
        catch
        {
            await context.Publish(new TransportReservationRejected(context.Message.Order.OrderId, null), context.CancellationToken);
            _logger.LogInformation($"Reservation rejected (OrderId={context.Message.Order.OrderId})");
        }
    }
}