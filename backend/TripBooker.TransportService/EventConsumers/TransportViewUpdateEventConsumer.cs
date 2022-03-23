using MassTransit;
using Newtonsoft.Json;
using TripBooker.TransportService.Model.Events;
using TripBooker.TransportService.Repositories;

namespace TripBooker.TransportService.EventConsumers;

internal class TransportViewUpdateEventConsumer : IConsumer<TransportViewUpdateEvent>
{
    private readonly ILogger<TransportViewUpdateEventConsumer> _logger;
    private readonly IEventTimestampRepository _timestampRepository;
    private readonly ITransportEventRepository _transportRepository;
    private readonly ITransportViewRepository _viewRepository;

    public TransportViewUpdateEventConsumer(
        ILogger<TransportViewUpdateEventConsumer> logger,
        IEventTimestampRepository timestampRepository, 
        ITransportEventRepository transportRepository, 
        ITransportViewRepository viewRepository)
    {
        _logger = logger;
        _timestampRepository = timestampRepository;
        _transportRepository = transportRepository;
        _viewRepository = viewRepository;
    }

    public async Task Consume(ConsumeContext<TransportViewUpdateEvent> context)
    {
        var cancellationToken = context.CancellationToken;

        var oldTimestamp = await UpdateTimestamp(cancellationToken);

        // read all events after timestamp
        var newEvents = await _transportRepository.GetEventsSinceAsync(oldTimestamp, cancellationToken);
        var transportIdsToUpdate = newEvents.Select(x => x.StreamId).Distinct().ToList();

        // update view and publish events
        foreach (var transportId in transportIdsToUpdate)
        {
            var transportModel = TransportBuilder.Build(
                await _transportRepository.GetTransportEventsAsync(transportId, cancellationToken));

            await _viewRepository.AddOrUpdateAsync(transportModel, cancellationToken);
            // TODO: publish event with updated data for other services' views
        }

        _logger.LogInformation($"Finished consuming events since {oldTimestamp}. " +
                               $"Updated rows (count={transportIdsToUpdate.Count}): {JsonConvert.SerializeObject(transportIdsToUpdate)}");
    }

    private async Task<DateTime> UpdateTimestamp(CancellationToken cancellationToken)
    {
        const string timestampKey = TransportViewUpdateEventConstants.TimestampKey;

        var eventTimestamp = await _timestampRepository.QueryOne(timestampKey, cancellationToken);
        var oldTimestamp = eventTimestamp?.Timestamp ?? DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
        _logger.LogInformation($"Started consuming events since {oldTimestamp}.");

        var newTimestamp = DateTime.UtcNow;
        if (eventTimestamp == null)
        {
            await _timestampRepository.CreateOne(timestampKey, newTimestamp, cancellationToken);
        }
        else
        {
            eventTimestamp.Timestamp = newTimestamp;
            await _timestampRepository.UpdateOne(eventTimestamp, cancellationToken);
        }

        return oldTimestamp;
    }
}