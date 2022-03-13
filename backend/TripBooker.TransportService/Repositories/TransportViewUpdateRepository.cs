using TripBooker.TransportService.Infrastructure;
using TripBooker.TransportService.Model;

namespace TripBooker.TransportService.Repositories;

internal interface ITransportViewUpdateRepository
{
    Task AddNewTransportAsync(TransportView transport, CancellationToken cancellationToken);
}

internal class TransportViewUpdateRepository : ITransportViewUpdateRepository
{
    private readonly SqlDbContext _dbContext;

    public TransportViewUpdateRepository(SqlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddNewTransportAsync(TransportView transport, CancellationToken cancellationToken)
    {
        await _dbContext.TransportView.AddAsync(transport, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}