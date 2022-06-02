using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TripBooker.Common.Hubs;
using TripBooker.WebApi.Hubs;

namespace TripBooker.WebApi.Consumers;

internal class PurchasedOfferNotificationConsumer : IConsumer<PurchasedOfferNotification>
{
    private readonly IHubContext<PurchasedOfferNotificationHub, IPurchasedOfferNotificationClient> _hubContext;
    private readonly ILogger<PurchasedOfferNotificationConsumer> _logger;

    public PurchasedOfferNotificationConsumer(
        IHubContext<PurchasedOfferNotificationHub, IPurchasedOfferNotificationClient> hubContext,
        ILogger<PurchasedOfferNotificationConsumer> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PurchasedOfferNotification> context)
    {
        _logger.LogInformation("Sending notifications of purchased offer for hotel days: " +
                               $"{JsonConvert.SerializeObject(context.Message)}");
        await _hubContext.Clients.All.SendNotification(context.Message);
    }
}