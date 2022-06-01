using MassTransit;
using Microsoft.EntityFrameworkCore;
using TripBooker.Common.Statistics.Updates;
using TripBooker.StatisticsService.Repository;

namespace TripBooker.StatisticsService.Services;

internal interface ITransportStatisticsService
{
    Task<TransportCounts> GetForDestination(
        string destination, CancellationToken cancellationToken);

    Task UpdateCount(string destination, CancellationToken cancellationToken);
}

internal class TransportStatisticsService : ITransportStatisticsService
{
    private readonly IBus _bus;
    private readonly ILogger<TransportStatisticsService> _logger;
    private readonly IReservationRepository _repository;

    public TransportStatisticsService(
        ILogger<TransportStatisticsService> logger,
        IReservationRepository repository,
        IBus bus)
    {
        _logger = logger;
        _repository = repository;
        _bus = bus;
    }

    public async Task<TransportCounts> GetForDestination(
        string destination, CancellationToken cancellationToken)
    {
        var transports = await _repository.QueryAll()
            .Where(x => x.DestinationAirportCode == destination)
            .GroupBy(x => x.DepartureAirportCode)
            .Select(x => new TransportCount(
                x.Key, x.Count())
            )
            .ToListAsync(cancellationToken);

        var returnTransports = await _repository.QueryAll()
            .Where(x => x.DestinationAirportCode == destination)
            .GroupBy(x => x.ReturnAirportCode)
            .Select(x => new TransportCount(
                x.Key, x.Count())
            )
            .ToListAsync(cancellationToken);

        return new TransportCounts(destination, transports, returnTransports);
    }

    public async Task UpdateCount(string destination, CancellationToken cancellationToken)
    {
        var newCounts = await GetForDestination(destination, cancellationToken);
        await _bus.Publish(newCounts, cancellationToken);
        _logger.LogInformation($"New transport counters for {destination} destination");
    }
}