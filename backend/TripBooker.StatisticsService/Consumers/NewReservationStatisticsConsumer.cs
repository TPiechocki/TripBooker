using MassTransit;
using TripBooker.Common.Statistics;

namespace TripBooker.StatisticsService.Consumers;

internal class NewReservationStatisticsConsumer : IConsumer<NewReservationEvent>
{
    private readonly ILogger<NewReservationStatisticsConsumer> _logger;

    public NewReservationStatisticsConsumer(ILogger<NewReservationStatisticsConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<NewReservationEvent> context)
    {
        _logger.LogInformation($"Storing statistics for order with id={context.Message.OrderId}");

        return Task.CompletedTask;
    }
}