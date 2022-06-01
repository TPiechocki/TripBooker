using Quartz;
using TripBooker.StatisticsService.Repository;
using TripBooker.StatisticsService.Services;

namespace TripBooker.StatisticsService.Infrastructure;

internal class StatisticsTimeoutJob : IJob
{
    private readonly IDestinationStatisticsService _destinationService;
    private readonly IHotelStatisticsService _hotelService;
    private readonly ILogger<StatisticsTimeoutJob> _logger;
    private readonly TimeSpan _range = TimeSpan.FromMinutes(15);
    private readonly IReservationRepository _repository;
    private readonly ITransportStatisticsService _transportService;

    public StatisticsTimeoutJob(
        IReservationRepository repository,
        IDestinationStatisticsService destinationService,
        ILogger<StatisticsTimeoutJob> logger,
        IHotelStatisticsService hotelService,
        ITransportStatisticsService transportService)
    {
        _repository = repository;
        _destinationService = destinationService;
        _logger = logger;
        _hotelService = hotelService;
        _transportService = transportService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var expiredStatistics =
            await _repository.RemoveOlderThan(DateTime.UtcNow - _range, context.CancellationToken);

        if (expiredStatistics.Any())
            _logger.LogInformation($"Statistics for {expiredStatistics.Count} orders have expired.");

        var destinations = expiredStatistics.Select(x => x.DestinationAirportCode)
            .Distinct().ToList();
        var hotelCodes = expiredStatistics.Select(x => new
        {
            Destination = x.DestinationAirportCode,
            x.HotelCode
        }).Distinct();

        var tasks = destinations.Select(x => _destinationService.UpdateCount(
            x, context.CancellationToken)).ToList();
        tasks.AddRange(hotelCodes.Select(x => _hotelService.UpdateCount(
            x.Destination, x.HotelCode, context.CancellationToken)));
        tasks.AddRange(destinations.Select(x => _transportService.UpdateCount(
            x, context.CancellationToken)));

        await Task.WhenAll(tasks);
    }
}