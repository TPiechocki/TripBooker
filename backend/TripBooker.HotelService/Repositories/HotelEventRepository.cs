using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TripBooker.HotelService.Infrastructure;
using TripBooker.HotelService.Model.Events;
using TripBooker.HotelService.Model.Events.Hotel;

namespace TripBooker.HotelService.Repositories;

internal interface IHotelEventRepository
{
    Task AddNewAsync(NewHotelDayEventData hotelEvent, CancellationToken cancellationToken);

    Task AddAsync(OccupatonUpdateEvent occupationUpdateEvent, Guid streamId, int previousVersion,
        CancellationToken cancellationToken, bool updateView = true);

    Task AddNewRangeAsync(IEnumerable<NewHotelDayEventData> hotelEventDatas, CancellationToken cancellationToken);

    Task AddToManyAsync(OccupatonUpdateEvent occupationUpdateEvent, IEnumerable<Guid> ids, IEnumerable<int> versions, CancellationToken cancellationToken);

    Task<ICollection<HotelEvent>> GetHotelEventsAsync(Guid streamId, CancellationToken cancellationToken);

    Task<ICollection<HotelEvent>> QueryAll(CancellationToken cancellationToken);

    Task<ICollection<HotelEvent>> GetEventsSinceAsync(DateTime timestamp, CancellationToken cancellationToken);
}

internal class HotelEventRepository : IHotelEventRepository
{
    private readonly HotelDbContext _dbContext;
    private readonly ILogger<HotelEventRepository> _logger;
    private readonly IBus _bus;

    public HotelEventRepository(
        HotelDbContext dbContext,
        ILogger<HotelEventRepository> logger,
        IBus bus)
    {
        _dbContext = dbContext;
        _logger = logger;
        _bus = bus;
    }

    public async Task AddNewAsync(NewHotelDayEventData hotelEvent, CancellationToken cancellationToken)
    {
        var guid = Guid.NewGuid();
        await _dbContext.HotelEvent.AddAsync(new HotelEvent(
            guid, 1, nameof(NewHotelDayEventData), hotelEvent),
            cancellationToken);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add a new HotelDay: {JsonConvert.SerializeObject(hotelEvent)}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }
    }

    public async Task AddNewRangeAsync(IEnumerable<NewHotelDayEventData> hotelEventDatas, CancellationToken cancellationToken)
    {
        var hotelEvents = new List<HotelEvent>();

        foreach(var hotelEvent in hotelEventDatas)
        {
            var guid = Guid.NewGuid();
            hotelEvents.Add(new HotelEvent(guid, 1, nameof(NewHotelDayEventData), hotelEvent));
        }

        _dbContext.HotelEvent.AddRange(hotelEvents);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add range of HotelDays";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }
    }

    public async Task AddAsync(OccupatonUpdateEvent occupationUpdateEvent, Guid streamId, int previousVersion, CancellationToken cancellationToken, bool updateView = true)
    {
        await _dbContext.HotelEvent.AddAsync(new HotelEvent(
                streamId, previousVersion + 1, nameof(OccupatonUpdateEvent), occupationUpdateEvent),
            cancellationToken);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add a occupation update event: {JsonConvert.SerializeObject(occupationUpdateEvent)}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }

        if(updateView) await _bus.Publish(new OccupationViewUpdateEvent(), cancellationToken);
    }

    public async Task<ICollection<HotelEvent>> GetHotelEventsAsync(Guid streamId, CancellationToken cancellationToken)
    {
        return await _dbContext.HotelEvent
            .Where(x => x.StreamId == streamId)
            .OrderBy(x => x.Version)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<HotelEvent>> GetEventsSinceAsync(DateTime timestamp, CancellationToken cancellationToken)
    {
        return await _dbContext.HotelEvent
            .Where(x => x.Timestamp > timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<HotelEvent>> QueryAll(CancellationToken cancellationToken)
    {
        return await _dbContext.HotelEvent.Select(x => x).ToListAsync(cancellationToken);
    }

    public async Task AddToManyAsync(OccupatonUpdateEvent occupationUpdateEvent, IEnumerable<Guid> ids, IEnumerable<int> versions, CancellationToken cancellationToken)
    {
        foreach(var occupation in ids.Zip(versions, Tuple.Create))
            await AddAsync(occupationUpdateEvent, occupation.Item1, occupation.Item2, cancellationToken, false);

        await _bus.Publish(new OccupationViewUpdateEvent(), cancellationToken);
    }
}
