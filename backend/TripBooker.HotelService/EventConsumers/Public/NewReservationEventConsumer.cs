using MassTransit;
using TripBooker.Common;
using TripBooker.Common.Hotel.Contract.Command;
using TripBooker.Common.Hotel.Contract.Response;
using TripBooker.HotelService.Services;

namespace TripBooker.HotelService.EventConsumers.Public;

internal class NewReservationEventConsumer : IConsumer<NewReservationContract>
{
    private readonly IHotelReservationService _reservationService;

    public NewReservationEventConsumer(IHotelReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public async Task Consume(ConsumeContext<NewReservationContract> context)
    {
        var result =
            await _reservationService.AddNewReservation(context.Message, context.CancellationToken);

        if (result.Status == ReservationStatus.Accepted)
        {
            await context.Publish(new ReservationAcceptedContract(context.Message.CorrelationId), context.CancellationToken);
        }
        else
        {
            await context.Publish(new ReservationRejectedContract(context.Message.CorrelationId), context.CancellationToken);
        }
    }
}
