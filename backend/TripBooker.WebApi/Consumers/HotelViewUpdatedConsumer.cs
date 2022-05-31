using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TripBooker.Common.Hubs;
using TripBooker.WebApi.Hubs;

namespace TripBooker.WebApi.Consumers;

internal class HotelViewUpdatedConsumer : IConsumer<HotelViewUpdated>
{
    private readonly IHubContext<OfferUpdatesHub, IOfferUpdatesClient> _hubContext;
    private readonly ILogger<HotelViewUpdatedConsumer> _logger;

    public HotelViewUpdatedConsumer(
        IHubContext<OfferUpdatesHub, IOfferUpdatesClient> hubContext,
        ILogger<HotelViewUpdatedConsumer> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<HotelViewUpdated> context)
    {
        _logger.LogInformation("Sending hotel update notifications: " +
                               $"{JsonConvert.SerializeObject(context.Message)}");
        await _hubContext.Clients.All.HotelUpdatedNotification(context.Message);
    }
}