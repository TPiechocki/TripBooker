using MassTransit;
using TripBooker.Common.Order.Payment;
using TripBooker.StatisticsService.Repository;
using TripBooker.StatisticsService.Services;

namespace TripBooker.StatisticsService.Consumers;

internal class PaymentAcceptedStatisticsConsumer : IConsumer<PaymentAccepted>
{
    private readonly IDestinationStatisticsService _destinationService;
    private readonly ILogger<PaymentAcceptedStatisticsConsumer> _logger;
    private readonly IReservationRepository _repository;

    public PaymentAcceptedStatisticsConsumer(
        ILogger<PaymentAcceptedStatisticsConsumer> logger,
        IReservationRepository repository,
        IDestinationStatisticsService destinationService)
    {
        _logger = logger;
        _repository = repository;
        _destinationService = destinationService;
    }

    public async Task Consume(ConsumeContext<PaymentAccepted> context)
    {
        _logger.LogInformation("Updating statistics timestamp for order with " +
                               $"id={context.Message.CorrelationId}");

        var destinationCode =
            await _repository.UpdateTimeStamp(context.Message.CorrelationId, context.CancellationToken);

        await _destinationService.UpdateCount(destinationCode, context.CancellationToken);
    }
}