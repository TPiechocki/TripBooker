using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TripBooker.TransportService.Infrastructure;
using TripBooker.TransportService.Model;

namespace TripBooker.TransportService.Repositories;

internal interface ITransportCommandRepository
{
    Task AddAsync(Transport transport, CancellationToken cancellationToken);
}

internal class TransportRepository : ITransportCommandRepository
{
    private readonly SqlDbContext _dbContext;
    private readonly ILogger<TransportRepository> _logger;

    public TransportRepository(SqlDbContext dbContext, ILogger<TransportRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task AddAsync(Transport transport, CancellationToken cancellationToken)
    {
        await _dbContext.Transport.AddAsync(transport, cancellationToken);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add a new Transport: {JsonConvert.SerializeObject(transport)}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }
    }
}