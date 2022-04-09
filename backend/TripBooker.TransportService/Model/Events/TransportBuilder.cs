using Newtonsoft.Json;
using TripBooker.Common;
using TripBooker.TransportService.Model.Events.Transport;

namespace TripBooker.TransportService.Model.Events;

internal static class TransportBuilder
{
    public static TransportModel Build(IEnumerable<BaseEvent> events)
    {
        var item = new TransportModel();

        foreach (var @event in events)
        {
            switch (@event.Type)
            {
                case nameof(NewTransportEventData):
                    item.ApplyNew(@event);
                    break;

                case nameof(TransportPlaceUpdateEvent):
                    item.ApplyPlaceUpdate(@event);
                    break;
            }
        }

        return item;
    }

    private static void ApplyNew(this TransportModel item, BaseEvent @event)
    {
        var data = JsonConvert.DeserializeObject<NewTransportEventData>(@event.Data)!;

        item.Id = @event.StreamId;
        item.Version = @event.Version;

        item.AvailablePlaces = data.AvailablePlaces;
        item.DepartureDate = data.DepartureDate;
        item.TransportOptionId = data.TransportOptionId;
    }

    private static void ApplyPlaceUpdate(this TransportModel item, BaseEvent @event)
    {
        var data = JsonConvert.DeserializeObject<TransportPlaceUpdateEvent>(@event.Data)!;

#if DEBUG
        if (item.AvailablePlaces + data.PlacesDelta != data.NewPlaces)
        {
            throw new InvalidDataException($"Inconsistent {nameof(TransportPlaceUpdateEvent)}: " +
                                           $"{JsonConvert.SerializeObject(data)} ");
        }
#endif

        item.Version = @event.Version;

        item.AvailablePlaces = data.NewPlaces;
    }
}