using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TripBooker.Common.Statistics.Updates;
using TripBooker.WebApi.Hubs;

namespace TripBooker.WebApi.Consumers;

internal class HotelCountUpdateConsumer : IConsumer<HotelCount>
{
    private readonly IHubContext<HotelStatisticsHub, IHotelStatisticsClient> _hubContext;
    private readonly ILogger<HotelCountUpdateConsumer> _logger;

    public HotelCountUpdateConsumer(
        IHubContext<HotelStatisticsHub, IHotelStatisticsClient> hubContext,
        ILogger<HotelCountUpdateConsumer> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<HotelCount> context)
    {
        _logger.LogInformation("Sending hotel count update: " +
                               $"{JsonConvert.SerializeObject(context.Message)}");
        await _hubContext.Clients.All.HotelCountUpdate(context.Message);
    }
}