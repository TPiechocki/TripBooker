using MongoDB.Driver;
using TripBooker.TransportService.Model;

namespace TripBooker.TransportService.Repositories;

internal interface ITransportReservationsRepository
{
    Task<IEnumerable<TransportReservations>> QueryAll(CancellationToken cancellationToken);
    Task<TransportReservations?> QueryOne(int id, CancellationToken cancellationToken);
    Task CreateOne(int transportId, CancellationToken cancellationToken);
}

internal class TransportReservationsRepository : ITransportReservationsRepository
{
    private readonly IMongoCollection<TransportReservations> _reservations;
    private readonly ILogger<TransportReservationsRepository> _logger;

    public TransportReservationsRepository(ILogger<TransportReservationsRepository> logger, IMongoDatabase mongoDatabase)
    {
        _logger = logger;
        _reservations = mongoDatabase.GetCollection<TransportReservations>("transport_reservations");
    }

    public async Task<IEnumerable<TransportReservations>> QueryAll(CancellationToken cancellationToken)
    {
        return await _reservations.AsQueryable().ToListAsync(cancellationToken);
    }

    public async Task<TransportReservations?> QueryOne(int id, CancellationToken cancellationToken)
    {
        var filter = Builders<TransportReservations>.Filter.Eq("_id", id);
        var cursor = await _reservations.FindAsync(filter, cancellationToken: cancellationToken)
            .WaitAsync(cancellationToken);

        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task CreateOne(int transportId, CancellationToken cancellationToken)
    {
        await _reservations.InsertOneAsync(new TransportReservations
        {
            TransportId = transportId,
            Reservations = new List<TransportReservation>()
        }, new InsertOneOptions(), cancellationToken);
    }
}