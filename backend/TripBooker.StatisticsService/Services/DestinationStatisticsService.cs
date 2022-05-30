using MassTransit;
using Microsoft.EntityFrameworkCore;
using TripBooker.Common.Statistics;
using TripBooker.StatisticsService.Repository;

namespace TripBooker.StatisticsService.Services;

internal interface IDestinationStatisticsService
{
    Task<IReadOnlyDictionary<string, int>> GetForAll(CancellationToken cancellationToken);
    Task<int> GetForOne(string destination, CancellationToken cancellationToken);
    Task UpdateCount(string destinationCode, CancellationToken cancellationToken);
}

internal class DestinationStatisticsService : IDestinationStatisticsService
{
    private readonly IBus _bus;
    private readonly ILogger<DestinationStatisticsService> _logger;
    private readonly IReservationRepository _repository;

    public DestinationStatisticsService(IReservationRepository repository,
        IBus bus, ILogger<DestinationStatisticsService> logger)
    {
        _repository = repository;
        _bus = bus;
        _logger = logger;
    }

    public async Task<IReadOnlyDictionary<string, int>> GetForAll(CancellationToken cancellationToken)
    {
        return await _repository.QueryAll().GroupBy(x => x.DestinationAirportCode)
            .Select(x => new
            {
                Destination = x.Key,
                Count = x.Count()
            })
            .ToDictionaryAsync(x => x.Destination, x => x.Count, cancellationToken);
    }

    public async Task<int> GetForOne(string destination, CancellationToken cancellationToken)
    {
        return await _repository.QueryAll().Where(x => x.DestinationAirportCode.Equals(destination))
            .CountAsync(cancellationToken);
    }

    public async Task UpdateCount(string destinationCode, CancellationToken cancellationToken)
    {
        var destinationCount = await GetForOne(destinationCode, cancellationToken);
        await _bus.Publish(new DestinationCountUpdate(destinationCode, destinationCount), cancellationToken);
        _logger.LogInformation($"New counter for {destinationCode}: {destinationCount}");
    }
}