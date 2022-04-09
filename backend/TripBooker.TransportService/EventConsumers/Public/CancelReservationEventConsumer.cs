using MassTransit;
using TripBooker.Common.Transport.Contract.Command;
using TripBooker.TransportService.Services;

namespace TripBooker.TransportService.EventConsumers.Public;

internal class CancelReservationEventConsumer : IConsumer<CancelReservationContract>
{
    private readonly ITransportReservationService _reservationService;

    public CancelReservationEventConsumer(ITransportReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public async Task Consume(ConsumeContext<CancelReservationContract> context)
    {
        await _reservationService.Cancel(context.Message.ReservationId, context.CancellationToken);
    }
}