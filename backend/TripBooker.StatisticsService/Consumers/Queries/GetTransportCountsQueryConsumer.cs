using MassTransit;
using TripBooker.Common.Statistics.Query;
using TripBooker.StatisticsService.Services;

namespace TripBooker.StatisticsService.Consumers.Queries;

internal class GetTransportCountsQueryConsumer : IConsumer<GetTransportCountsQuery>
{
    private readonly ILogger<GetTransportCountsQueryConsumer> _logger;
    private readonly ITransportStatisticsService _transportService;

    public GetTransportCountsQueryConsumer(
        ITransportStatisticsService transportService,
        ILogger<GetTransportCountsQueryConsumer> logger)
    {
        _transportService = transportService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GetTransportCountsQuery> context)
    {
        _logger.LogInformation("Transport counts query received.");

        var counts = await _transportService.GetForDestination(
            context.Message.Destination, context.CancellationToken);

        await context.RespondAsync(counts);

        _logger.LogInformation("Transport counts query handled.");
    }
}