using MassTransit;
using TripBooker.Common.Order.Hotel;
using TripBooker.HotelService.Services;

namespace TripBooker.HotelService.EventConsumers.Public;

internal class CancelReservationEventConsumer : IConsumer<CancelHotelReservation>
{
    private readonly IHotelReservationService _reservationService;

    public CancelReservationEventConsumer(IHotelReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public async Task Consume(ConsumeContext<CancelHotelReservation> context)
    {
        await _reservationService.Cancel(context.Message.ReservationId, context.CancellationToken);
    }
}
