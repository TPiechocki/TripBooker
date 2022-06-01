using MassTransit;
using Newtonsoft.Json;
using TripBooker.Common.Order;

namespace TripBooker.TourOperator.EventConsumers.Public;

public class TourOperatorReportConsumer : IConsumer<TourOperatorReport>
{
    private readonly ILogger<TourOperatorReportConsumer> _logger;

    public TourOperatorReportConsumer(ILogger<TourOperatorReportConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<TourOperatorReport> context)
    {
        _logger.LogInformation(
            $"Order report received: {JsonConvert.SerializeObject(context.Message, Formatting.Indented)}");

        return Task.CompletedTask;
    }
}