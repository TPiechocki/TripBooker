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

    public NewHotelReservationEventConsumer(
        IHotelReservationService reservationService,
        IBus bus)
    {
        _reservationService = reservationService;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<NewHotelReservation> context)
    {
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
            }
            else
            {
                await context.Publish(new HotelReservationRejected(context.Message.Order.OrderId, result.Id), context.CancellationToken);
            }
        }
        catch
        {
            await context.Publish(new HotelReservationRejected(context.Message.Order.OrderId, null), context.CancellationToken);
        }
    }
}
