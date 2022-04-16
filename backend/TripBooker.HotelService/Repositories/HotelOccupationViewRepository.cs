using Microsoft.EntityFrameworkCore;
using TripBooker.HotelService.Infrastructure;
using TripBooker.HotelService.Model;

namespace TripBooker.HotelService.Repositories;

internal interface IHotelOccupationViewRepository
{
    Task AddAsync(HotelOccupationModel occupationModel, CancellationToken cancellationToken);

    Task<HotelOccupationModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<HotelOccupationModel>> QueryAllAsync(CancellationToken cancellationToken);

    Task<HotelOccupationModel?> GetByHotelIdAndDateAsync(Guid hotelId, DateTime day, CancellationToken cancellationToken);

    Task<ICollection<HotelOccupationModel>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken);

    Task AddOrUpdateAsync(HotelOccupationModel occupationModel, CancellationToken cancellationToken);
}

internal class HotelOccupationViewRepository : IHotelOccupationViewRepository
{
    private readonly HotelDbContext _dbContext;

    public HotelOccupationViewRepository(HotelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(HotelOccupationModel occupationModel, CancellationToken cancellationToken)
    {
        await _dbContext.HotelOccupationView.AddAsync(occupationModel, cancellationToken);
    }

    public async Task<HotelOccupationModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.HotelOccupationView.FindAsync(cancellationToken, id);
    }

    public async Task<IEnumerable<HotelOccupationModel>> QueryAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.HotelOccupationView.Select(x => x).ToListAsync(cancellationToken);
    }

    public async Task<ICollection<HotelOccupationModel>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken)
    {
        return await _dbContext.HotelOccupationView
            .Where(om => om.HotelId == hotelId)
            .ToListAsync(cancellationToken);
    }

    public async Task<HotelOccupationModel?> GetByHotelIdAndDateAsync(Guid hotelId, DateTime day, CancellationToken cancellationToken)
    {
        return await _dbContext.HotelOccupationView
            .Where(om => om.HotelId == hotelId && om.Date == day)
            .FirstAsync(cancellationToken);
    }

    public async Task AddOrUpdateAsync(HotelOccupationModel occupationModel, CancellationToken cancellationToken)
    {
        if (await _dbContext.HotelOccupationView.AnyAsync(x => x.Id == occupationModel.Id, cancellationToken))
        {
            _dbContext.HotelOccupationView.Update(occupationModel);
        }
        else
        {
            await AddAsync(occupationModel, cancellationToken);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
