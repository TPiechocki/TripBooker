using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TripBooker.TransportService.Infrastructure;
using TripBooker.TransportService.Model;

namespace TripBooker.TransportService.Repositories;

internal interface ITransportViewUpdateRepository
{
    Task AddNewTransportAsync(TransportView transport, CancellationToken cancellationToken);
}

internal class TransportViewUpdateRepository : ITransportViewUpdateRepository
{
    private readonly SqlDbContext _dbContext;
    private readonly ILogger<TransportViewUpdateRepository> _logger;

    public TransportViewUpdateRepository(SqlDbContext dbContext, ILogger<TransportViewUpdateRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task AddNewTransportAsync(TransportView transport, CancellationToken cancellationToken)
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
}