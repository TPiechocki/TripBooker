using MassTransit;
using Microsoft.EntityFrameworkCore;
using TripBooker.Common.Statistics.Updates;
using TripBooker.StatisticsService.Repository;

namespace TripBooker.StatisticsService.Services;

internal interface IHotelStatisticsService
{
    Task<IReadOnlyDictionary<string, int>> GetOrderCountsForDestination(
        string destination, CancellationToken cancellationToken);

    Task<HotelCount> GetForOne(string destination, string hotelCode,
        CancellationToken cancellationToken);

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

    public async Task<IReadOnlyDictionary<string, int>> GetOrderCountsForDestination(
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
        var hotelCount = await GetForOne(destination, hotelCode, cancellationToken);
        await _bus.Publish(new HotelCount(destination, hotelCode, hotelCount.OrderCount,
            hotelCount.RoomsStudio, hotelCount.RoomsSmall, hotelCount.RoomsMedium, hotelCount.RoomsLarge,
            hotelCount.RoomsApartment), cancellationToken);
        _logger.LogInformation($"New counter for {hotelCode} hotel: {hotelCount.OrderCount}");
    }

    public async Task<HotelCount> GetForOne(string destination, string hotelCode,
        CancellationToken cancellationToken)
    {
        return await _repository.QueryAll().Where(x => x.HotelCode.Equals(hotelCode))
            .GroupBy(x => x.HotelCode)
            .Select(x => new HotelCount(destination, x.Key, 
                x.Count(), x.Sum(h => h.RoomsStudio), x.Sum(h => h.RoomsSmall), 
                x.Sum(h => h.RoomsMedium), x.Sum(h => h.RoomsLarge), 
                x.Sum(h => h.RoomsApartment)))
            .SingleOrDefaultAsync(cancellationToken) 
            ?? new HotelCount(destination, hotelCode, 0, 0, 0, 0, 0, 0);
    }
}