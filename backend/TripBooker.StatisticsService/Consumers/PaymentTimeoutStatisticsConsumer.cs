using MassTransit;
using TripBooker.Common.Order.Payment;
using TripBooker.StatisticsService.Repository;
using TripBooker.StatisticsService.Services;

namespace TripBooker.StatisticsService.Consumers;

internal class PaymentTimeoutStatisticsConsumer : IConsumer<PaymentTimeout>
{
    private readonly IDestinationStatisticsService _destinationService;
    private readonly ILogger<PaymentTimeoutStatisticsConsumer> _logger;
    private readonly IReservationRepository _repository;

    public PaymentTimeoutStatisticsConsumer(
        ILogger<PaymentTimeoutStatisticsConsumer> logger,
        IReservationRepository repository,
        IDestinationStatisticsService destinationService)
    {
        _logger = logger;
        _repository = repository;
        _destinationService = destinationService;
    }

    public async Task Consume(ConsumeContext<PaymentTimeout> context)
    {
        _logger.LogInformation("Removing order with reject payment from statistics " +
                               $"(id={context.Message.CorrelationId})");

        var destinationCode =
            await _repository.Remove(context.Message.CorrelationId, context.CancellationToken);

        await _destinationService.UpdateCount(destinationCode, context.CancellationToken);
    }
}