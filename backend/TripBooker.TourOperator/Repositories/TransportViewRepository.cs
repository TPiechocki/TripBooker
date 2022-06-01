using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TripBooker.TourOperator.Infrastructure;
using TripBooker.TourOperator.Model;

namespace TripBooker.TourOperator.Repositories;

internal interface ITransportViewRepository
{
    Task AddOrUpdateAsync(TransportModel transport, CancellationToken cancellationToken);

    Task AddOrUpdateManyAsync(IEnumerable<TransportModel> transports, CancellationToken cancellationToken);

    IEnumerable<TransportModel> QueryAll();
}

internal class TransportViewRepository : ITransportViewRepository
{
    private readonly TourOperatorDbContext _dbContext;
    private readonly ILogger<TransportViewRepository> _logger;

    public TransportViewRepository(TourOperatorDbContext dbContext, ILogger<TransportViewRepository> logger)
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
        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add a new TransportView: {JsonConvert.SerializeObject(transport)}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }
    }

    public async Task AddOrUpdateManyAsync(IEnumerable<TransportModel> transports, CancellationToken cancellationToken)
    {
        if (!transports.Any())
            return;

        _dbContext.TransportView.UpdateRange(transports);

        foreach (var transport in transports)
        {
            if (await _dbContext.TransportView.AnyAsync(x => x.Id == transport.Id, cancellationToken))
            {
                _dbContext.TransportView.Update(transport);
                await _dbContext.SaveChangesAsync(cancellationToken);

                _dbContext.Entry(transport).State = EntityState.Detached;
            }
            else
            {
                await AddAsync(transport, cancellationToken);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task AddAsync(TransportModel transport, CancellationToken cancellationToken)
    {
        await _dbContext.TransportView.AddAsync(transport, cancellationToken);
    }

    public IEnumerable<TransportModel> QueryAll()
    {
        return _dbContext.TransportView.Select(x => x).ToList();
    }
}
