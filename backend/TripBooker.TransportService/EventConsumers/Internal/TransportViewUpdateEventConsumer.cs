using MassTransit;
using Newtonsoft.Json;
using TripBooker.Common.Transport.Contract;
using TripBooker.TransportService.Model.Events;
using TripBooker.TransportService.Model.Mappings;
using TripBooker.TransportService.Repositories;

namespace TripBooker.TransportService.EventConsumers.Internal;

internal class TransportViewUpdateEventConsumer : IConsumer<TransportViewUpdateEvent>
{
    private readonly ILogger<TransportViewUpdateEventConsumer> _logger;
    private readonly IEventTimestampRepository _timestampRepository;
    private readonly ITransportEventRepository _transportRepository;
    private readonly ITransportViewRepository _viewRepository;
    private readonly IBus _bus;
    private readonly ITransportOptionRepository _transportOptionRepository;

    public TransportViewUpdateEventConsumer(
        ILogger<TransportViewUpdateEventConsumer> logger,
        IEventTimestampRepository timestampRepository, 
        ITransportEventRepository transportRepository, 
        ITransportViewRepository viewRepository, 
        IBus bus,
        ITransportOptionRepository transportOptionRepository)
    {
        _logger = logger;
        _timestampRepository = timestampRepository;
        _transportRepository = transportRepository;
        _viewRepository = viewRepository;
        _bus = bus;
        _transportOptionRepository = transportOptionRepository;
    }

    public async Task Consume(ConsumeContext<TransportViewUpdateEvent> context)
    {
        var cancellationToken = context.CancellationToken;

        const string timestampKey = TransportViewUpdateEventConstants.TimestampKey;

        // query current timestamp
        var eventTimestamp =  await _timestampRepository.QueryOne(timestampKey, cancellationToken);
        var oldTimestamp = eventTimestamp?.Timestamp ?? DateTime.MinValue;
        oldTimestamp = DateTime.SpecifyKind(oldTimestamp, DateTimeKind.Utc);

        var newTimestamp = await ConsumeEventsSince(oldTimestamp, cancellationToken);
        
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

    private async Task<DateTime> ConsumeEventsSince(DateTime oldTimestamp, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Started consuming events since {oldTimestamp}.");

        // read all events after old timestamp
        var newEvents = await _transportRepository.GetEventsSinceAsync(oldTimestamp, cancellationToken);
        var transportIdsToUpdate = newEvents.Select(x => x.StreamId).Distinct().ToList();

        // update view and publish events
        var updates = new List<TransportViewContract>();
        foreach (var transportId in transportIdsToUpdate)
        {
            var transportModel = TransportBuilder.Build(
                await _transportRepository.GetTransportEventsAsync(transportId, cancellationToken));

            await _viewRepository.AddOrUpdateAsync(transportModel, cancellationToken);

            var transportOption = await _transportOptionRepository.GetById(transportModel.TransportOptionId);
            if (transportOption == null)
            {
                throw new InvalidOperationException("Cannot map transport without defined transport option." +
                                                    $"(missingOptionId={transportModel.TransportOptionId}");
            }

            updates.Add(TransportViewContractMapper.MapFrom(transportModel, transportOption));
        }
        await _bus.PublishBatch(updates, cancellationToken);

        _logger.LogInformation($"Finished consuming events since {oldTimestamp}. " +
                               $"Updated rows (count={transportIdsToUpdate.Count}): {JsonConvert.SerializeObject(transportIdsToUpdate)}");
        return newEvents.Count == 0
            ? oldTimestamp
            : DateTime.SpecifyKind(newEvents.Select(x => x.Timestamp).Max(), DateTimeKind.Utc);
    }
}