using MassTransit;
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

    public TransportCommandRepository(SqlDbContext dbContext, IBus bus)
    {
        _dbContext = dbContext;
        _bus = bus;
    }

    public void AddTransport(Transport transport)
    {
        var task = AddTransportAsync(transport, default);
        task.GetAwaiter().GetResult();
    }

    public async Task AddTransportAsync(Transport transport, CancellationToken cancellationToken)
    {
        await _dbContext.Transport.AddAsync(transport, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

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