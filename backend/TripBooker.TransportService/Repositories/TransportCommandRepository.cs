using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TripBooker.TransportService.Infrastructure;
using TripBooker.TransportService.Model;
using TripBooker.TransportService.Model.Events;

namespace TripBooker.TransportService.Repositories;

internal interface ITransportCommandRepository
{
    void AddTransport(Transport transport);

    Task AddTransportAsync(Transport transport, CancellationToken cancellationToken);
}

internal class TransportCommandRepository : ITransportCommandRepository
{
    private readonly SqlDbContext _dbContext;
    private readonly IBus _bus;
    private readonly ILogger<TransportCommandRepository> _logger;

    public TransportCommandRepository(SqlDbContext dbContext, IBus bus, ILogger<TransportCommandRepository> logger)
    {
        _dbContext = dbContext;
        _bus = bus;
        _logger = logger;
    }

    public void AddTransport(Transport transport)
    {
        var task = AddTransportAsync(transport, default);
        task.GetAwaiter().GetResult();
    }

    public async Task AddTransportAsync(Transport transport, CancellationToken cancellationToken)
    {
        await _dbContext.Transport.AddAsync(transport, cancellationToken);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add a new Transport: {JsonConvert.SerializeObject(transport)}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }

        await PublishNewTransportEvent(transport, cancellationToken);
    }

    private async Task PublishNewTransportEvent(Transport transport, CancellationToken cancellationToken)
    {
        string destination;
        string departure;
        if (transport.IsReturn)
        {
            departure = transport.Option.Destination;
            destination = transport.Option.DeparturePlace;
        }
        else
        {
            departure = transport.Option.DeparturePlace;
            destination = transport.Option.Destination;
        }

        await _bus.Publish(new NewTransportEvent(transport.Id, transport.DepartureDate,
                departure, destination, transport.Option.Type,
                transport.NumberOfSeats),
            cancellationToken);
    }
}