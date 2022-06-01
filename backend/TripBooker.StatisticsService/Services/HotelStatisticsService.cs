using MassTransit;
using Microsoft.EntityFrameworkCore;
using TripBooker.Common.Statistics.Updates;
using TripBooker.StatisticsService.Repository;

namespace TripBooker.StatisticsService.Services;

internal interface IHotelStatisticsService
{
    Task<IReadOnlyDictionary<string, int>> GetForDestination(
        string destination, CancellationToken cancellationToken);

    Task UpdateCount(string destination, string hotelCode, CancellationToken cancellationToken);
}

internal class HotelStatisticsService : IHotelStatisticsService
{
    private readonly IBus _bus;
    private readonly ILogger<HotelStatisticsService> _logger;
    private readonly IReservationRepository _repository;

    public HotelStatisticsService(
        ILogger<HotelStatisticsService> logger,
        IReservationRepository repository,
        IBus bus)
    {
        _logger = logger;
        _repository = repository;
        _bus = bus;
    }

    public async Task<IReadOnlyDictionary<string, int>> GetForDestination(
        string destination, CancellationToken cancellationToken)
    {
        return await _repository.QueryAll()
            .Where(x => x.DestinationAirportCode == destination)
            .GroupBy(x => x.HotelCode)
            .Select(x => new
            {
                HotelCode = x.Key,
                Count = x.Count()
            })
            .ToDictionaryAsync(x => x.HotelCode, x => x.Count, cancellationToken);
    }

    public async Task UpdateCount(string destination, string hotelCode, CancellationToken cancellationToken)
    {
        var destinationCount = await GetForOne(hotelCode, cancellationToken);
        await _bus.Publish(new HotelCountUpdate(destination, hotelCode, destinationCount), cancellationToken);
        _logger.LogInformation($"New counter for {hotelCode} hotel: {destinationCount}");
    }

    private async Task<int> GetForOne(string hotelCode, CancellationToken cancellationToken)
    {
        return await _repository.QueryAll().Where(x => x.HotelCode.Equals(hotelCode))
            .CountAsync(cancellationToken);
    }
}