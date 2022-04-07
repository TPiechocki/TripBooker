using Microsoft.EntityFrameworkCore;
using TripBooker.HotelService.Infrastructure;
using TripBooker.HotelService.Model;

namespace TripBooker.HotelService.Repositories;

internal interface IHotelOptionRepository
{
    Task AddAsyc(HotelOption hotelOption, CancellationToken cancellationToken);

    Task<HotelOption?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<ICollection<HotelOption>> QuerryAllAsync(CancellationToken cancellationToken);
}

internal class HotelOptionRepository : IHotelOptionRepository
{
    private readonly HotelDbContext _dbContext;

    public HotelOptionRepository(HotelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsyc(HotelOption hotelOption, CancellationToken cancellationToken)
    {
        await _dbContext.HotelOption.AddAsync(hotelOption, cancellationToken);
    }

    public async Task<HotelOption?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Retrieves Hotel with the Rooms loaded
        return await _dbContext.HotelOption.Include(h => h.Rooms).FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<ICollection<HotelOption>> QuerryAllAsync(CancellationToken cancellationToken)
    {
        // Retrieves Hotels with the Rooms loaded
        return await _dbContext.HotelOption.Include(h => h.Rooms).ToListAsync(cancellationToken);
    }
}
