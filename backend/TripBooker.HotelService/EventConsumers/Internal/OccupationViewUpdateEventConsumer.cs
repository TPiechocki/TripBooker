using MassTransit;
using Newtonsoft.Json;
using TripBooker.Common.Hotel.Contract;
using TripBooker.HotelService.Infrastructure;
using TripBooker.HotelService.Model;
using TripBooker.HotelService.Model.Events;
using TripBooker.HotelService.Model.Mappings;
using TripBooker.HotelService.Repositories;

namespace TripBooker.HotelService.EventConsumers.Internal;

internal class OccupationViewUpdateEventConsumer : IConsumer<OccupationViewUpdateEvent>
{
    private readonly ILogger<OccupationViewUpdateEventConsumer> _logger;
    private readonly IEventTimestampRepository _timestampRepository;
    private readonly IHotelEventRepository _hotelRepository;
    private readonly IHotelOccupationViewRepository _viewRepository;
    private readonly IBus _bus;
    private readonly IHotelOptionRepository _hotelOptionRepository;

    public OccupationViewUpdateEventConsumer(
        ILogger<OccupationViewUpdateEventConsumer> logger,
        IEventTimestampRepository timestampRepository,
        IHotelEventRepository hotelRepository,
        IHotelOccupationViewRepository viewRepository,
        IBus bus,
        IHotelOptionRepository hotelOptionRepository)
    {
        _logger = logger;
        _timestampRepository = timestampRepository;
        _hotelRepository = hotelRepository;
        _viewRepository = viewRepository;
        _bus = bus;
        _hotelOptionRepository = hotelOptionRepository;
    }

    public async Task Consume(ConsumeContext<OccupationViewUpdateEvent> context)
    {
        var cancellationToken = context.CancellationToken;

        const string timestampKey = OccupationViewUpdateEventConstants.TimestampKey;

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
        var newEvents = await _hotelRepository.GetEventsSinceAsync(oldTimestamp, cancellationToken);
        newEvents = newEvents.Take(5000).ToList();
        var newViewsEvents = newEvents.Where(x => x.Version == 1).ToDictionary(x => x.StreamId);
        var hotelDayIdsToUpdate = newEvents.Select(x => x.StreamId).Distinct().Except(newViewsEvents.Keys).ToList();
        
        // get data of all hotels for easy access
        var hotels = await _hotelOptionRepository.QuerryAllAsync(cancellationToken);

        // update view and publish events
        foreach (var hotelDayId in hotelDayIdsToUpdate)
        {
            var occupationModel = HotelOccupationBuilder.Build(
                await _hotelRepository.GetHotelEventsAsync(hotelDayId, cancellationToken));

            await _viewRepository.AddOrUpdateAsync(occupationModel, cancellationToken);

            var hotelOption = hotels.First(h => h.Id == occupationModel.HotelId);
            if (hotelOption == null)
            {
                throw new InvalidOperationException("Cannot map occupation without defined Hotel option." +
                                                    $"(missingOptionId={occupationModel.HotelId}");
            }

            await _bus.Publish(HotelOccupationViewContractMapper.MapFrom(occupationModel, hotelOption), cancellationToken);
        }

        // create new views and publish events
        var newDataChunks = newViewsEvents.Values.Chunk(1000);

        foreach (var chunk in newDataChunks)
        {
            await CreateNewData(chunk, hotels, cancellationToken);
        }

        _logger.LogInformation($"Finished consuming events since {oldTimestamp}. " +
                               $"Updated rows (count={hotelDayIdsToUpdate.Count}): {JsonConvert.SerializeObject(hotelDayIdsToUpdate)}");

        return newEvents.Count == 0
            ? oldTimestamp
            : DateTime.SpecifyKind(newEvents.Select(x => x.Timestamp).Max(), DateTimeKind.Utc);
    }

    private async Task CreateNewData(IEnumerable<HotelEvent> newViewsEvents, ICollection<HotelOption> hotels, CancellationToken cancellationToken)
    {
        var newViews = new List<HotelOccupationModel>();
        var newContracts = new List<HotelOccupationViewContract>();
        foreach (var newHotelDay in newViewsEvents)
        {
            var occupationModel = HotelOccupationBuilder.Build(new List<HotelEvent>() {newHotelDay});
            newViews.Add(occupationModel);

            var hotelOption = hotels.First(h => h.Id == occupationModel.HotelId);
            if (hotelOption == null)
            {
                throw new InvalidOperationException("Cannot map occupation without defined Hotel option." +
                                                    $"(missingOptionId={occupationModel.HotelId}");
            }

            newContracts.Add(HotelOccupationViewContractMapper.MapFrom(occupationModel, hotelOption));
        }

        if (newViews.Any()) await _viewRepository.AddOrUpdateManyAsync(newViews, cancellationToken);
        if (newContracts.Any())
        {
            await _bus.PublishBatch(newContracts, cancellationToken);
        }
    }
}
