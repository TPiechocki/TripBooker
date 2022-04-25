using MongoDB.Driver;
using TripBooker.Common;

namespace TripBooker.HotelService.Repositories;

internal interface IEventTimestampRepository
{
    Task CreateOne(string id, DateTime timestamp, CancellationToken cancellationToken);

    Task<EventTimestamp?> QueryOne(string id, CancellationToken cancellationToken);

    Task UpdateOne(EventTimestamp entity, CancellationToken cancellationToken);
}

internal class EventTimestampRepository : IEventTimestampRepository
{
    private readonly IMongoCollection<EventTimestamp> _timestamps;

    public EventTimestampRepository(IMongoDatabase mongoDatabase)
    {
        _timestamps = mongoDatabase.GetCollection<EventTimestamp>("hotel_event_timestamps");

        _timestamps.Indexes.CreateOne(
            new CreateIndexModel<EventTimestamp>(Builders<EventTimestamp>.IndexKeys.Ascending(x => x.Id)));
    }

    public async Task CreateOne(string id, DateTime timestamp, CancellationToken cancellationToken)
    {
        await _timestamps.InsertOneAsync(new EventTimestamp
        {
            Id = id,
            Timestamp = timestamp
        }, new InsertOneOptions(), cancellationToken);
    }

    public async Task<EventTimestamp?> QueryOne(string id, CancellationToken cancellationToken)
    {
        var filter = Builders<EventTimestamp>.Filter.Eq(x => x.Id, id);
        var cursor = await _timestamps.FindAsync(filter, cancellationToken: cancellationToken)
            .WaitAsync(cancellationToken);

        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateOne(EventTimestamp entity, CancellationToken cancellationToken)
    {
        var filter = Builders<EventTimestamp>.Filter.Eq(x => x.BsonId, entity.BsonId);
        await _timestamps.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
    }
}
