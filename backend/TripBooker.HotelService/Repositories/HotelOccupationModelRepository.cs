using Microsoft.EntityFrameworkCore;
using TripBooker.HotelService.Infrastructure;
using TripBooker.HotelService.Model;

namespace TripBooker.HotelService.Repositories;

internal interface IHotelOccupationModelRepository
{
    Task AddAsync(HotelOccupationModel occupationModel, CancellationToken cancellationToken);

    Task<HotelOccupationModel?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<HotelOccupationModel?> GetByHotelIdAndDateAsync(int hotelId, DateTime day, CancellationToken cancellationToken);

    Task<ICollection<HotelOccupationModel>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken);
}

internal class HotelOccupationModelRepository : IHotelOccupationModelRepository
{
    private readonly HotelDbContext _dbContext;

    public HotelOccupationModelRepository(HotelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(HotelOccupationModel occupationModel, CancellationToken cancellationToken)
    {
        await _dbContext.HotelOccupationModels.AddAsync(occupationModel, cancellationToken);
    }

    public async Task<HotelOccupationModel?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.HotelOccupationModels.FindAsync(cancellationToken, id);
    }

    public async Task<ICollection<HotelOccupationModel>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken)
    {
        return await _dbContext.HotelOccupationModels
            .Where(om => om.HotelId == hotelId)
            .ToListAsync(cancellationToken);
    }

    public async Task<HotelOccupationModel?> GetByHotelIdAndDateAsync(int hotelId, DateTime day, CancellationToken cancellationToken)
    {
        return await _dbContext.HotelOccupationModels
            .Where(om => om.HotelId == hotelId && om.Date == day)
            .FirstAsync(cancellationToken);
    }
}
