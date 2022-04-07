using AutoMapper;
using MassTransit;
using Newtonsoft.Json;
using TripBooker.Common.Transport.Contract;
using TripBooker.TransportService.Model.Events;
using TripBooker.TransportService.Repositories;

namespace TripBooker.TransportService.EventConsumers.Internal;

internal class TransportViewUpdateEventConsumer : IConsumer<TransportViewUpdateEvent>
{
    private readonly ILogger<TransportViewUpdateEventConsumer> _logger;
    private readonly IEventTimestampRepository _timestampRepository;
    private readonly ITransportEventRepository _transportRepository;
    private readonly ITransportViewRepository _viewRepository;
    private readonly IBus _bus;
    private readonly IMapper _mapper;

    public TransportViewUpdateEventConsumer(
        ILogger<TransportViewUpdateEventConsumer> logger,
        IEventTimestampRepository timestampRepository, 
        ITransportEventRepository transportRepository, 
        ITransportViewRepository viewRepository, 
        IBus bus, 
        IMapper mapper)
    {
        _logger = logger;
        _timestampRepository = timestampRepository;
        _transportRepository = transportRepository;
        _viewRepository = viewRepository;
        _bus = bus;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<TransportViewUpdateEvent> context)
    {
        var cancellationToken = context.CancellationToken;

        const string timestampKey = TransportViewUpdateEventConstants.TimestampKey;

        // query current timestamp
        var eventTimestamp = await _timestampRepository.QueryOne(timestampKey, cancellationToken);
        var oldTimestamp = eventTimestamp?.Timestamp ?? DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
        var newTimestamp = DateTime.UtcNow;

        await ConsumeEventsSince(oldTimestamp, cancellationToken);

        // save new timestamp after successful consume
        if (eventTimestamp == null)
        {
            await _timestampRepository.CreateOne(timestampKey, newTimestamp, cancellationToken);
        }
        else
        {
            eventTimestamp.Timestamp = newTimestamp;
            await _timestampRepository.UpdateOne(eventTimestamp, cancellationToken);
        }
    }

    private async Task ConsumeEventsSince(DateTime oldTimestamp, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Started consuming events since {oldTimestamp}.");

        // read all events after old timestamp
        var newEvents = await _transportRepository.GetEventsSinceAsync(oldTimestamp, cancellationToken);
        var transportIdsToUpdate = newEvents.Select(x => x.StreamId).Distinct().ToList();

        // update view and publish events
        foreach (var transportId in transportIdsToUpdate)
        {
            var transportModel = TransportBuilder.Build(
                await _transportRepository.GetTransportEventsAsync(transportId, cancellationToken));

            await _viewRepository.AddOrUpdateAsync(transportModel, cancellationToken);
            await _bus.Publish(_mapper.Map<TransportViewContract>(transportModel), cancellationToken);
        }

        _logger.LogInformation($"Finished consuming events since {oldTimestamp}. " +
                               $"Updated rows (count={transportIdsToUpdate.Count}): {JsonConvert.SerializeObject(transportIdsToUpdate)}");
    }
}