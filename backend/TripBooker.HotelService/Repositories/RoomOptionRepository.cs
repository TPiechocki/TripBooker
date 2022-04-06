using Microsoft.EntityFrameworkCore;
using TripBooker.HotelService.Infrastructure;
using TripBooker.HotelService.Model;

namespace TripBooker.HotelService.Repositories;

internal interface IRoomOptionRepository
{
    Task<RoomOption?> GetById(int id, CancellationToken cancellationToken);
}

internal class RoomOptionRepository : IRoomOptionRepository
{
    private readonly HotelDbContext _dbContext;

    public RoomOptionRepository(HotelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RoomOption?> GetById(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.RoomOption.FindAsync(id, cancellationToken);
    }
}
