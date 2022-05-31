using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TripBooker.TourOperator.Infrastructure;
using TripBooker.TourOperator.Model;

namespace TripBooker.TourOperator.Repositories;

internal interface IHotelOccupationViewRepository
{
    Task AddOrUpdateAsync(HotelOccupationModel hotelOccupation, CancellationToken cancellationToken);

    Task AddOrUpdateManyAsync(IEnumerable<HotelOccupationModel> hotelOccupation, CancellationToken cancellationToken);

    IEnumerable<HotelOccupationModel> QueryAll();
}

internal class HotelOccupationViewRepository : IHotelOccupationViewRepository
{
    private readonly TourOperatorDbContext _dbContext;
    private readonly ILogger<HotelOccupationViewRepository> _logger;

    public HotelOccupationViewRepository(TourOperatorDbContext dbContext, ILogger<HotelOccupationViewRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task AddOrUpdateAsync(HotelOccupationModel hotelOccupation, CancellationToken cancellationToken)
    {
        if (await _dbContext.HotelOccupationView.AnyAsync(x =>
                x.HotelId == hotelOccupation.HotelId && x.Date == hotelOccupation.Date, cancellationToken))
        {
            _dbContext.HotelOccupationView.Update(hotelOccupation);
        }
        else
        {
            await AddAsync(hotelOccupation, cancellationToken);
        }
        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add a new HotelOccupationView: {JsonConvert.SerializeObject(hotelOccupation)}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }
    }

    public async Task AddOrUpdateManyAsync(IEnumerable<HotelOccupationModel> hotelOccupations, CancellationToken cancellationToken)
    {
        if (!hotelOccupations.Any())
            return;

        foreach (var hotelOccupation in hotelOccupations)
        {
            if (await _dbContext.HotelOccupationView.AnyAsync(x =>
                    x.HotelId == hotelOccupation.HotelId && x.Date == hotelOccupation.Date, cancellationToken))
            {
                _dbContext.HotelOccupationView.Update(hotelOccupation);
                await _dbContext.SaveChangesAsync(cancellationToken);

                _dbContext.Entry(hotelOccupation).State = EntityState.Detached;
            }
            else
            {
                await AddAsync(hotelOccupation, cancellationToken);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task AddAsync(HotelOccupationModel hotelOccupation, CancellationToken cancellationToken)
    {
        await _dbContext.HotelOccupationView.AddAsync(hotelOccupation, cancellationToken);
    }

    public IEnumerable<HotelOccupationModel> QueryAll()
    {
        return _dbContext.HotelOccupationView.Select(x => x).ToList();
    }
}
