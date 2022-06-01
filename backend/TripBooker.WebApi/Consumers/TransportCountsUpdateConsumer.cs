using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TripBooker.Common.Statistics.Updates;
using TripBooker.WebApi.Hubs;

namespace TripBooker.WebApi.Consumers;

internal class TransportCountsUpdateConsumer : IConsumer<TransportCounts>
{
    private readonly IHubContext<TransportStatisticsHub, ITransportStatisticsClient> _hubContext;
    private readonly ILogger<TransportCountsUpdateConsumer> _logger;

    public TransportCountsUpdateConsumer(
        IHubContext<TransportStatisticsHub, ITransportStatisticsClient> hubContext,
        ILogger<TransportCountsUpdateConsumer> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TransportCounts> context)
    {
        _logger.LogInformation("Sending transport counts update: " +
                               $"{JsonConvert.SerializeObject(context.Message)}");
        await _hubContext.Clients.All.TransportCountsUpdate(context.Message);
    }
}