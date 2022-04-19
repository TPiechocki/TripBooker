using MassTransit;
using TripBooker.Common.Hotel.Contract.Command;
using TripBooker.HotelService.Services;

namespace TripBooker.HotelService.EventConsumers.Public;

internal class CancelReservationEventConsumer : IConsumer<CancelReservationContract>
{
    private readonly IHotelReservationService _reservationService;

    public CancelReservationEventConsumer(IHotelReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public async Task Consume(ConsumeContext<CancelReservationContract> context)
    {
        await _reservationService.Cancel(context.Message.ReservationId, context.CancellationToken);
    }
}
