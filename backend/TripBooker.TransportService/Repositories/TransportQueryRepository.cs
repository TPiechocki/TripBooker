using Microsoft.EntityFrameworkCore;
using TripBooker.TransportService.Infrastructure;
using TripBooker.TransportService.Model;

namespace TripBooker.TransportService.Repositories;

internal interface ITransportQueryRepository
{
    Task<IEnumerable<TransportView>> QueryAllAsync(CancellationToken cancellationToken);
}

internal class TransportQueryRepository : ITransportQueryRepository
{
    private readonly SqlDbContext _dbContext;

    public TransportQueryRepository(SqlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TransportView>> QueryAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.TransportView.Select(x => x).ToListAsync(cancellationToken);
    }
}