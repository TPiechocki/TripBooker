using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TripBooker.Common.Statistics.Updates;
using TripBooker.WebApi.Hubs;

namespace TripBooker.WebApi.Consumers;

internal class DestinationCountUpdateConsumer : IConsumer<DestinationCountUpdate>
{
    private readonly IHubContext<DestinationStatisticsHub, IDestinationStatisticsClient> _hubContext;
    private readonly ILogger<DestinationCountUpdateConsumer> _logger;

    public DestinationCountUpdateConsumer(
        IHubContext<DestinationStatisticsHub, IDestinationStatisticsClient> hubContext,
        ILogger<DestinationCountUpdateConsumer> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DestinationCountUpdate> context)
    {
        _logger.LogInformation("Sending destination count update: " +
                               $"{JsonConvert.SerializeObject(context.Message)}");
        await _hubContext.Clients.All.DestinationCountUpdate(context.Message);
    }
}