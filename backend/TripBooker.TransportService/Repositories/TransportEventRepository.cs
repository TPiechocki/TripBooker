using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TripBooker.TransportService.Infrastructure;
using TripBooker.TransportService.Model.Events.Transport;

namespace TripBooker.TransportService.Repositories;

internal interface ITransportEventRepository
{
    Task<Guid> AddNewAsync(NewTransportEventData transportEvent, CancellationToken cancellationToken);

    Task AddAsync(TransportPlaceUpdateEvent placeUpdateEvent, Guid streamId, int previousVersion,
        CancellationToken cancellationToken);

    Task<ICollection<TransportEvent>> GetTransportEvents(Guid streamId, CancellationToken cancellationToken);
}

internal class TransportEventRepository : ITransportEventRepository
{
    private readonly TransportDbContext _dbContext;
    private readonly ILogger<TransportEventRepository> _logger;

    public TransportEventRepository(
        TransportDbContext dbContext, 
        ILogger<TransportEventRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Guid> AddNewAsync(
        NewTransportEventData transportEvent,
        CancellationToken cancellationToken)
    {
        var guid = Guid.NewGuid();
        await _dbContext.TransportEvent.AddAsync(new TransportEvent(
            guid, 1, nameof(NewTransportEventData), transportEvent), 
            cancellationToken);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add a new Transport: {JsonConvert.SerializeObject(transportEvent)}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }

        return guid;
    }

    public async Task AddAsync(
        TransportPlaceUpdateEvent placeUpdateEvent,
        Guid streamId,
        int previousVersion,
        CancellationToken cancellationToken)
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
    }

    public async Task<ICollection<TransportEvent>> GetTransportEvents(Guid streamId, CancellationToken cancellationToken)
    {
        return await _dbContext.TransportEvent
            .Where(x => x.StreamId == streamId)
            .OrderBy(x => x.Timestamp)
            .ToListAsync(cancellationToken);
    }
}