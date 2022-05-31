using MassTransit;
using TripBooker.Common.Order.Payment;
using TripBooker.StatisticsService.Repository;

namespace TripBooker.StatisticsService.Consumers;

internal class PaymentAcceptedStatisticsConsumer : IConsumer<PaymentAccepted>
{
    private readonly ILogger<PaymentAcceptedStatisticsConsumer> _logger;
    private readonly IReservationRepository _repository;

    public PaymentAcceptedStatisticsConsumer(
        ILogger<PaymentAcceptedStatisticsConsumer> logger,
        IReservationRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<PaymentAccepted> context)
    {
        _logger.LogInformation("Updating statistics timestamp for order with " +
                               $"id={context.Message.CorrelationId}");

        await _repository.UpdateTimeStamp(context.Message.CorrelationId, context.CancellationToken);
    }
}