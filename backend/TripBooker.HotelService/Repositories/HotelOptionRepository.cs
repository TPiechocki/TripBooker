using TripBooker.HotelService.Infrastructure;
using TripBooker.HotelService.Model;

namespace TripBooker.HotelService.Repositories;

internal interface IHotelOptionRepository
{
    Task AddAsyc(HotelOption hotelOption, CancellationToken cancellationToken);
    Task<HotelOption?> GetById(int id, CancellationToken cancellationToken);
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

    public async Task<HotelOption?> GetById(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.HotelOption.FindAsync(id, cancellationToken);
    }
}
