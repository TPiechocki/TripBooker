using MassTransit;
using TripBooker.Common.Statistics.Updates;
using TripBooker.StatisticsService.Services;

namespace TripBooker.StatisticsService.Consumers.Queries;

internal class GetHotelCountConsumer : IConsumer<GetHotelCount>
{
    private readonly IHotelStatisticsService _hotelService;
    private readonly ILogger<GetHotelCountConsumer> _logger;

    public GetHotelCountConsumer(
        IHotelStatisticsService hotelService,
        ILogger<GetHotelCountConsumer> logger)
    {
        _hotelService = hotelService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GetHotelCount> context)
    {
        _logger.LogInformation("Hotel count query received.");

        var result = await _hotelService
            .GetForOne(context.Message.Destination, context.Message.HotelCode, context.CancellationToken);

        await context.RespondAsync(result);

        _logger.LogInformation("Hotel count query handled.");
    }
}