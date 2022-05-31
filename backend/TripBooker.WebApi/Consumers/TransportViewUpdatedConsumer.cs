using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TripBooker.Common.Hubs;
using TripBooker.WebApi.Hubs;

namespace TripBooker.WebApi.Consumers;

internal class TransportViewUpdatedConsumer : IConsumer<TransportViewUpdated>
{
    private readonly IHubContext<OfferUpdatesHub, IOfferUpdatesClient> _hubContext;
    private readonly ILogger<TransportViewUpdatedConsumer> _logger;

    public TransportViewUpdatedConsumer(
        IHubContext<OfferUpdatesHub, IOfferUpdatesClient> hubContext,
        ILogger<TransportViewUpdatedConsumer> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TransportViewUpdated> context)
    {
        _logger.LogInformation("Sending transport update notifications: " +
                               $"{JsonConvert.SerializeObject(context.Message)}");
        await _hubContext.Clients.All.TransportsUpdatedNotification(context.Message);
    }
}