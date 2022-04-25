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
        var newEvents = await _hotelRepository.GetEventsSinceAsync(oldTimestamp, cancellationToken);
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

            HotelOption hotelOption = hotels.Where(h => h.Id == occupationModel.HotelId).First();
            if (hotelOption == null)
            {
                throw new InvalidOperationException("Cannot map occupation without defined Hotel option." +
                                                    $"(missingOptionId={occupationModel.HotelId}");
            }

            await _bus.Publish(HotelOccupationViewContractMapper.MapFrom(occupationModel, hotelOption), cancellationToken);
        }

        // create new views and publish events
        var newViews = new List<HotelOccupationModel>();
        var newContracts = new List<HotelOccupationViewContract>();
        foreach (var newHotelDay in newViewsEvents.Values)
        {
            var occupationModel = HotelOccupationBuilder.Build(new List<HotelEvent>() { newHotelDay });
            newViews.Add(occupationModel);

            HotelOption hotelOption = hotels.Where(h => h.Id == occupationModel.HotelId).First();
            if (hotelOption == null)
            {
                throw new InvalidOperationException("Cannot map occupation without defined Hotel option." +
                                                    $"(missingOptionId={occupationModel.HotelId}");
            }

            newContracts.Add(HotelOccupationViewContractMapper.MapFrom(occupationModel, hotelOption));
        }
        if (newViews.Any()) await _viewRepository.AddManyAsync(newViews, cancellationToken);
        if (newContracts.Any())
        {
            var chunks = newContracts.Chunk(1000);

            // add in chunks so data is partially available earlier
            foreach (var chunk in chunks)
            {
                await _bus.PublishBatch(chunk, cancellationToken);
            }
            
        }

        _logger.LogInformation($"Finished consuming events since {oldTimestamp}. " +
                               $"Updated rows (count={hotelDayIdsToUpdate.Count}): {JsonConvert.SerializeObject(hotelDayIdsToUpdate)}");
    }

}
