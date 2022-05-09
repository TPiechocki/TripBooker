using MassTransit;
using TripBooker.Common;
using TripBooker.Common.Order.Hotel;
using TripBooker.HotelService.Model.Events;
using TripBooker.HotelService.Services;

namespace TripBooker.HotelService.EventConsumers.Public;

internal class NewHotelReservationEventConsumer : IConsumer<NewHotelReservation>
{
    private readonly IHotelReservationService _reservationService;
    private readonly IBus _bus;
    private readonly ILogger<NewHotelReservationEventConsumer> _logger;

    public NewHotelReservationEventConsumer(
        IHotelReservationService reservationService,
        IBus bus, 
        ILogger<NewHotelReservationEventConsumer> logger)
    {
        _reservationService = reservationService;
        _bus = bus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NewHotelReservation> context)
    {
        _logger.LogInformation($"New reservation received (OrderId={context.Message.Order.OrderId})");
        try
        {
            var result = 
                await _reservationService.AddNewReservation(context.Message, context.CancellationToken);

            if (result.Status == ReservationStatus.Accepted)
            {
                await context.Publish(
                    new HotelReservationAccepted(context.Message.Order.OrderId, result.Price, result.Id), 
                    context.CancellationToken);
                await _bus.Publish(new OccupationViewUpdateEvent(), context.CancellationToken);
                _logger.LogInformation($"Reservation accepted (OrderId={context.Message.Order.OrderId})");
            }
            else
            {
                await context.Publish(new HotelReservationRejected(context.Message.Order.OrderId, result.Id), context.CancellationToken);
                _logger.LogInformation($"Reservation rejected (OrderId={context.Message.Order.OrderId})");
            }
        }
        catch
        {
            await context.Publish(new HotelReservationRejected(context.Message.Order.OrderId, null), context.CancellationToken);
            _logger.LogInformation($"Reservation rejected (OrderId={context.Message.Order.OrderId})");
        }
    }
}
