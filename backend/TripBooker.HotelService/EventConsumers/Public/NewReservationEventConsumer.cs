using MassTransit;
using TripBooker.Common;
using TripBooker.Common.Hotel.Contract.Command;
using TripBooker.Common.Hotel.Contract.Response;
using TripBooker.HotelService.Model.Events;
using TripBooker.HotelService.Services;

namespace TripBooker.HotelService.EventConsumers.Public;

internal class NewReservationEventConsumer : IConsumer<NewReservationContract>
{
    private readonly IHotelReservationService _reservationService;
    private readonly IBus _bus;

    public NewReservationEventConsumer(
        IHotelReservationService reservationService,
        IBus bus)
    {
        _reservationService = reservationService;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<NewReservationContract> context)
    {
        try
        {
            var result = await _reservationService.AddNewReservation(context.Message, context.CancellationToken);

            if (result.Status == ReservationStatus.Accepted)
            {
                await context.Publish(new ReservationAcceptedContract(context.Message.CorrelationId, result.Id), context.CancellationToken);
                await _bus.Publish(new OccupationViewUpdateEvent(), context.CancellationToken);
            }
            else
            {
                await context.Publish(new ReservationRejectedContract(context.Message.CorrelationId, result.Id), context.CancellationToken);
            }
        }
        catch
        {
            await context.Publish(new ReservationRejectedContract(context.Message.CorrelationId, null), context.CancellationToken);
        }
    }
}
