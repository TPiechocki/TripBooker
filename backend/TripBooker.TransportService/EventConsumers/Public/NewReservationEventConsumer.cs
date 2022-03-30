using MassTransit;
using TripBooker.Common.Transport;
using TripBooker.Common.Transport.Contract.Command;
using TripBooker.Common.Transport.Contract.Response;
using TripBooker.TransportService.Services;

namespace TripBooker.TransportService.EventConsumers.Public;

internal class NewReservationEventConsumer : IConsumer<NewReservationContract>
{
    private readonly ITransportReservationService _reservationService;

    public NewReservationEventConsumer(ITransportReservationService reservationService)
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