using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TripBooker.TransportService.Infrastructure;
using TripBooker.TransportService.Model;

namespace TripBooker.TransportService.Repositories;

internal interface ITransportViewRepository
{
    Task AddAsync(TransportModel transport, CancellationToken cancellationToken);

    Task<TransportModel?> QueryByIdAsync(int id, CancellationToken cancellationToken);

    Task<IEnumerable<TransportModel>> QueryAllAsync(CancellationToken cancellationToken);

    Task AddOrUpdateAsync(TransportModel transport, CancellationToken cancellationToken);
}

internal class TransportViewRepository : ITransportViewRepository
{
    private readonly TransportDbContext _dbContext;
    private readonly ILogger<TransportViewRepository> _logger;

    public TransportViewRepository(TransportDbContext dbContext, ILogger<TransportViewRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task AddAsync(TransportModel transport, CancellationToken cancellationToken)
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

    public async Task<TransportModel?> QueryByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.TransportView.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<TransportModel>> QueryAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.TransportView.Select(x => x).ToListAsync(cancellationToken);
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
}