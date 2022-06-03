using MassTransit;
using TripBooker.Common.TourOperator.Contract;
using TripBooker.Common.TourOperator.Contract.Query;
using TripBooker.TourOperator.Model.Extensions;
using TripBooker.TourOperator.Repositories;

namespace TripBooker.TourOperator.EventConsumers.Public;

internal class TransportUpdateQueryConsumer : IConsumer<TransportUpdateQuery>
{
    private readonly ITransportViewRepository _transportRepository;
    private readonly IUpdatesRepository _updatesRepository;
    private readonly ILogger<TransportUpdateQueryConsumer> _logger;
    private readonly IBus _bus;

    public TransportUpdateQueryConsumer(
        ITransportViewRepository transportRepository,
        IUpdatesRepository updatesRepository,
        ILogger<TransportUpdateQueryConsumer> logger,
        IBus bus)
    {
        _transportRepository = transportRepository;
        _updatesRepository = updatesRepository;
        _logger = logger;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<TransportUpdateQuery> context)
    {
        _logger.LogInformation("Query for transport update received");

        var transport = await _transportRepository.GetByIdAsync(context.Message.Id, context.CancellationToken);

        if (transport == null)
        {
            _logger.LogError($"Could not locate transport with Id = {context.Message.Id}");
            return;
        }

        var update = new TransportUpdateContract()
        {
            Id = transport.Id,
            AvailablePlacesChange = Math.Max(context.Message.AvailablePlacesChange, -transport.AvailablePlaces),
            PriceChangedFlag = transport.TicketPrice != context.Message.NewTicketPrice,
            NewTicketPrice = context.Message.NewTicketPrice
        };

        await _bus.Publish(update, context.CancellationToken);
        await _updatesRepository.AddAsync(update.Describe(transport.TicketPrice), context.CancellationToken);

        _logger.LogInformation($"Query for transport update consumed, update for tranpsort with Id = {context.Message.Id}");
    }
}
