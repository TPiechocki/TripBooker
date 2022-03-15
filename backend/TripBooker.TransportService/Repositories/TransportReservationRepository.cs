using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TripBooker.TransportService.Infrastructure;
using TripBooker.TransportService.Model;

namespace TripBooker.TransportService.Repositories;

internal interface ITransportReservationRepository
{
    Task AddAsync(TransportReservation reservation, CancellationToken cancellationToken);
}

internal class TransportReservationRepository : ITransportReservationRepository
{
    private readonly SqlDbContext _dbContext;
    private readonly ILogger<TransportRepository> _logger;

    public TransportReservationRepository(
        SqlDbContext dbContext, 
        ILogger<TransportRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task AddAsync(TransportReservation reservation, CancellationToken cancellationToken)
    {
        await _dbContext.TransportReservation.AddAsync(reservation, cancellationToken);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add a new Transport: {JsonConvert.SerializeObject(reservation)}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }
    }
}