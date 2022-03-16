using TripBooker.TransportService.Infrastructure;
using TripBooker.TransportService.Model;

namespace TripBooker.TransportService.Repositories;

internal interface ITransportOptionRepository
{
    Task<TransportOption?> GetById(int id);
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
}