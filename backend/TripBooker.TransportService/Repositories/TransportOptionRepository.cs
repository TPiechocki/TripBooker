using Microsoft.EntityFrameworkCore;
using TripBooker.TransportService.Infrastructure;
using TripBooker.TransportService.Model;

namespace TripBooker.TransportService.Repositories;

internal interface ITransportOptionRepository
{
    Task<TransportOption?> GetById(int id);

    Task<IEnumerable<TransportOption>> GetByIds(IEnumerable<int> ids, CancellationToken cancellationToken);
}

internal class TransportOptionRepository : ITransportOptionRepository
{
    private readonly TransportDbContext _dbContext;

    public TransportOptionRepository(TransportDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TransportOption?> GetById(int id)
    {
        return await _dbContext.TransportOption.FindAsync(id);
    }

    public async Task<IEnumerable<TransportOption>> GetByIds(IEnumerable<int> ids, CancellationToken cancellationToken)
    {
        return await _dbContext.TransportOption
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }
}