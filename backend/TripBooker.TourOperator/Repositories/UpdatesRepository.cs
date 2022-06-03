using Microsoft.EntityFrameworkCore;
using TripBooker.TourOperator.Infrastructure;
using TripBooker.TourOperator.Model;

namespace TripBooker.TourOperator.Repositories;

internal interface IUpdatesRepository
{
    Task AddAsync(string description, CancellationToken cancellationToken);

    Task<IEnumerable<UpdateModel>> QueryLast10Async(CancellationToken cancellationToken);
}

internal class UpdatesRepository : IUpdatesRepository
{
    private readonly TourOperatorDbContext _dbContext;

    public UpdatesRepository(TourOperatorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(string description, CancellationToken cancellationToken)
    {
        await _dbContext.UpdateModel.AddAsync(
            new UpdateModel(DateTime.UtcNow, description),
            cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<UpdateModel>> QueryLast10Async(CancellationToken cancellationToken)
    {
        return await _dbContext.UpdateModel.Select(u => u)
                                           .OrderByDescending(u => u.Timestamp)
                                           .Take(10)
                                           .ToListAsync(cancellationToken);
    }
}
