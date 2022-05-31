using MassTransit;
using TripBooker.Common.Statistics;
using TripBooker.StatisticsService.Services;

namespace TripBooker.StatisticsService.Consumers;

internal class GetDestinationCountsQueryConsumer : IConsumer<GetDestinationCountsQuery>
{
    private readonly IDestinationStatisticsService _destinationService;
    private readonly ILogger<GetDestinationCountsQueryConsumer> _logger;


    public GetDestinationCountsQueryConsumer(
        IDestinationStatisticsService destinationService,
        ILogger<GetDestinationCountsQueryConsumer> logger)
    {
        _destinationService = destinationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GetDestinationCountsQuery> context)
    {
        _logger.LogInformation("Destination counts query received.");

        var counts = await _destinationService.GetForAll(context.CancellationToken);

        await context.RespondAsync(new GetDestinationCountsResponse(counts));

        _logger.LogInformation("Destination counts query handled.");
    }
}