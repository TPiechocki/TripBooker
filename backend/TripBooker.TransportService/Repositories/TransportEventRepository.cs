using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TripBooker.TransportService.Infrastructure;
using TripBooker.TransportService.Model.Events;
using TripBooker.TransportService.Model.Events.Transport;

namespace TripBooker.TransportService.Repositories;

internal interface ITransportEventRepository
{
    Task AddAsync(TransportPlaceUpdateEvent placeUpdateEvent, Guid streamId, int previousVersion,
        CancellationToken cancellationToken, bool updateViews = true);

    Task AddAsync(TicketPriceUpdateEvent placeUpdateEvent, Guid streamId, int previousVersion,
        CancellationToken cancellationToken, bool updateViews = true);

    Task AddManyNewAsync(IEnumerable<NewTransportEventData> transportEvents, CancellationToken cancellationToken);

    Task<ICollection<TransportEvent>> GetTransportEventsAsync(Guid streamId, CancellationToken cancellationToken);

    Task<ICollection<TransportEvent>> GetEventsSinceAsync(DateTime timestamp, CancellationToken cancellationToken);
}

internal class TransportEventRepository : ITransportEventRepository
{
    private readonly TransportDbContext _dbContext;
    private readonly ILogger<TransportEventRepository> _logger;
    private readonly IBus _bus;

    public TransportEventRepository(
        TransportDbContext dbContext, 
        ILogger<TransportEventRepository> logger, 
        IBus bus)
    {
        _dbContext = dbContext;
        _logger = logger;
        _bus = bus;
    }

    public async Task AddManyNewAsync(
        IEnumerable<NewTransportEventData> transportEvents,
        CancellationToken cancellationToken)
    {
        var guid = Guid.NewGuid();
        await _dbContext.TransportEvent.AddRangeAsync(transportEvents.Select(x =>
                new TransportEvent(Guid.NewGuid(), 1, nameof(NewTransportEventData), x)),
            cancellationToken);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            const string message = "Could not add new Transports.";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }

        await _bus.Publish(new TransportViewUpdateEvent(), cancellationToken);
    }

    public async Task AddAsync(
        TransportPlaceUpdateEvent placeUpdateEvent,
        Guid streamId,
        int previousVersion,
        CancellationToken cancellationToken, 
        bool updateViews = true)
    {
        await _dbContext.TransportEvent.AddAsync(new TransportEvent(
                streamId, previousVersion+1, nameof(TransportPlaceUpdateEvent), placeUpdateEvent),
            cancellationToken);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add a transport place update event: {JsonConvert.SerializeObject(placeUpdateEvent)}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }

        if (updateViews) await _bus.Publish(new TransportViewUpdateEvent(), cancellationToken);
    }

    public async Task AddAsync(
        TicketPriceUpdateEvent priceUpdateEvent,
        Guid streamId,
        int previousVersion,
        CancellationToken cancellationToken, 
        bool updateViews = true)
    {
        await _dbContext.TransportEvent.AddAsync(new TransportEvent(
                streamId, previousVersion + 1, nameof(TicketPriceUpdateEvent), priceUpdateEvent),
            cancellationToken);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add a transport ticket price update event: {JsonConvert.SerializeObject(priceUpdateEvent)}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }

        if (updateViews) await _bus.Publish(new TransportViewUpdateEvent(), cancellationToken);
    }

    public async Task<ICollection<TransportEvent>> GetTransportEventsAsync(Guid streamId, CancellationToken cancellationToken)
    {
        return await _dbContext.TransportEvent
            .Where(x => x.StreamId == streamId)
            .OrderBy(x => x.Version)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<TransportEvent>> GetEventsSinceAsync(DateTime timestamp, CancellationToken cancellationToken)
    {
        return await _dbContext.TransportEvent
            .Where(x => x.Timestamp > timestamp)
            .ToListAsync(cancellationToken);
    }
}