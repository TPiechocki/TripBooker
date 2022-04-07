using Newtonsoft.Json;
using TripBooker.Common;
using TripBooker.HotelService.Model.Events.Hotel;

namespace TripBooker.HotelService.Model;

internal static class HotelOccupationBuilder
{
    public static HotelOccupationModel Build(IEnumerable<BaseEvent> events)
    {
        var item = new HotelOccupationModel();

        foreach (var @event in events)
        {
            switch (@event.Type)
            {
                case nameof(NewHotelDayEventData):
                    item.ApplyNew(@event);
                    break;

                case nameof(OccupatonUpdateEvent):
                    item.ApplyUpdate(@event);
                    break;
            }
        }

        return item;
    }

    private static void ApplyNew(this HotelOccupationModel item, BaseEvent @event)
    {
        var data = JsonConvert.DeserializeObject<NewHotelDayEventData>(@event.Data)!;

        item.Id = @event.StreamId;
        item.Version = @event.Version;

        item.HotelId = data.HotelId;
        item.Date = data.Date;
        item.RoomsStudio = data.RoomsStudio;
        item.RoomsSmall = data.RoomsSmall;
        item.RoomsMedium = data.RoomsMedium;
        item.RoomsLarge = data.RoomsLarge;
        item.RoomsApartment = data.RoomsApartment;
    }

    private static void ApplyUpdate(this HotelOccupationModel item, BaseEvent @event)
    {
        var data = JsonConvert.DeserializeObject<OccupatonUpdateEvent>(@event.Data)!;

        item.Version = @event.Version;

        item.RoomsStudio -= data.RoomsStudio;
        item.RoomsSmall -= data.RoomsSmall;
        item.RoomsMedium -= data.RoomsMedium;
        item.RoomsLarge -= data.RoomsLarge;
        item.RoomsApartment -= data.RoomsApartment;
    }
}
