using Quartz;
using TripBooker.StatisticsService.Repository;
using TripBooker.StatisticsService.Services;

namespace TripBooker.StatisticsService.Infrastructure;

internal class StatisticsTimeoutJob : IJob
{
    private readonly IDestinationStatisticsService _destinationService;

    private readonly ILogger<StatisticsTimeoutJob> _logger;
    private readonly TimeSpan _range = TimeSpan.FromMinutes(15);
    private readonly IReservationRepository _repository;

    public StatisticsTimeoutJob(
        IReservationRepository repository,
        IDestinationStatisticsService destinationService,
        ILogger<StatisticsTimeoutJob> logger)
    {
        _repository = repository;
        _destinationService = destinationService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var expiredStatistics =
            await _repository.RemoveOlderThan(DateTime.UtcNow - _range, context.CancellationToken);

        if (expiredStatistics.Count() > 0)
            _logger.LogInformation($"Statistics for {expiredStatistics.Count()} orders have expired.");

        var destinations = expiredStatistics.Select(x => x.DestinationAirportCode).Distinct();

        var tasks = destinations.Select(x => _destinationService.UpdateCount(x, context.CancellationToken));
        await Task.WhenAll(tasks);
    }
}