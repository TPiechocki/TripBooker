using MassTransit;
using TripBooker.Common.Statistics.Query;
using TripBooker.StatisticsService.Services;

namespace TripBooker.StatisticsService.Consumers.Queries;

internal class GetHotelCountsQueryConsumer : IConsumer<GetHotelCountsQuery>
{
    private readonly IHotelStatisticsService _hotelService;
    private readonly ILogger<GetHotelCountsQueryConsumer> _logger;


    public GetHotelCountsQueryConsumer(
        IHotelStatisticsService hotelService,
        ILogger<GetHotelCountsQueryConsumer> logger)
    {
        _hotelService = hotelService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GetHotelCountsQuery> context)
    {
        _logger.LogInformation("Hotel counts query received.");

        var counts = await _hotelService.GetForDestination(
            context.Message.Destination, context.CancellationToken);

        await context.RespondAsync(new GetHotelCountsResponse(counts));

        _logger.LogInformation("Hotel counts query handled.");
    }
}