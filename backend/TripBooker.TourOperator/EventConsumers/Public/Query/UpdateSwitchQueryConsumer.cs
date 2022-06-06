using MassTransit;
using TripBooker.Common.TourOperator.Contract.Query;

namespace TripBooker.TourOperator.EventConsumers.Public.Query;

internal class UpdateSwitchQueryConsumer : IConsumer<UpdateSwitchQuery>
{
    private readonly ILogger<UpdateSwitchQueryConsumer> _logger;

    public UpdateSwitchQueryConsumer(ILogger<UpdateSwitchQueryConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<UpdateSwitchQuery> context)
    {
        _logger.LogInformation("Query for update switch received.");

        if (context.Message.GenerateUpdates && File.Exists("StopUpdate"))
        {
            File.Delete("StopUpdate");
            _logger.LogInformation("Update generation resumed.");
        }

        if (!context.Message.GenerateUpdates && !File.Exists("StopUpdate"))
        {
            File.Create("StopUpdate");
            _logger.LogInformation("Update generation stopped.");
        }

        _logger.LogInformation("Query for update switch handled.");
        return Task.CompletedTask;
    }
}
