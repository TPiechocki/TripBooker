using MassTransit;
using TripBooker.Common.TourOperator.Contract;
using TripBooker.Common.TourOperator.Contract.Query;
using TripBooker.TourOperator.Repositories;

namespace TripBooker.TourOperator.EventConsumers.Public;

internal class TransportUpdateQueryConsumer : IConsumer<TransportUpdateQuery>
{
    private readonly ITransportViewRepository _repository;
    private readonly ILogger<TransportUpdateQueryConsumer> _logger;
    private readonly IBus _bus;

    public TransportUpdateQueryConsumer(
        ITransportViewRepository repository,
        ILogger<TransportUpdateQueryConsumer> logger,
        IBus bus)
    {
        _repository = repository;
        _logger = logger;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<TransportUpdateQuery> context)
    {
        _logger.LogInformation("Query for transport update received");

        var transport = await _repository.GetByIdAsync(context.Message.Id, context.CancellationToken);

        if (transport == null)
        {
            _logger.LogError($"Could not locate transport with Id = {context.Message.Id}");
            return;
        }

        await _bus.Publish(new TransportUpdateContract()
        {
            Id = transport.Id,
            AvailablePlacesChange = Math.Max(context.Message.AvailablePlacesChange, -transport.AvailablePlaces),
            PriceChangedFlag = transport.TicketPrice != context.Message.NewTicketPrice,
            NewTicketPrice = context.Message.NewTicketPrice
        }, context.CancellationToken);

        _logger.LogInformation($"Query for transport update consumed, update for tranpsort with Id = {context.Message.Id}");
    }
}
