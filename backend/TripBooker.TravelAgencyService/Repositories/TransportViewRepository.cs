using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TripBooker.TravelAgencyService.Infrastructure;
using TripBooker.TravelAgencyService.Model;

namespace TripBooker.TravelAgencyService.Repositories;

internal interface ITransportViewRepository
{
    Task AddOrUpdateAsync(TransportModel transport, CancellationToken cancellationToken);

    IQueryable<TransportModel> QueryAll();
}

internal class TransportViewRepository : ITransportViewRepository
{
    private readonly TravelAgencyDbContext _dbContext;
    private readonly ILogger<TransportViewRepository> _logger;

    public TransportViewRepository(TravelAgencyDbContext dbContext, ILogger<TransportViewRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task AddOrUpdateAsync(TransportModel transport, CancellationToken cancellationToken)
    {
        if (await _dbContext.TransportView.AnyAsync(x => x.Id == transport.Id, cancellationToken))
        {
            _dbContext.TransportView.Update(transport);
        }
        else
        {
            await AddAsync(transport, cancellationToken);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task AddAsync(TransportModel transport, CancellationToken cancellationToken)
    {
        await _dbContext.TransportView.AddAsync(transport, cancellationToken);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add a new TransportView: {JsonConvert.SerializeObject(transport)}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }
    }

    public IQueryable<TransportModel> QueryAll()
    {
        return _dbContext.TransportView.Select(x => x);
    }
}