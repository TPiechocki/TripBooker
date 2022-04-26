using Newtonsoft.Json;
using TripBooker.Common;

namespace TripBooker.HotelService.Model.Events;

internal static class ReservationBuilder
{
    public static ReservationModel Build(IEnumerable<BaseEvent> events)
    {
        var item = new ReservationModel();

        foreach (var @event in events)
        {
            switch (@event.Type)
            {
                case nameof(NewReservationEventData):
                    item.ApplyNew(@event);
                    break;

                case nameof(ReservationAcceptedEventData):
                    item.ApplyAccepted(@event);
                    break;

                case nameof(ReservationRejectedEventData):
                    item.ApplyRejected(@event);
                    break;
            }
        }

        return item;
    }

    private static void ApplyNew(this ReservationModel item, BaseEvent @event)
    {
        var data = JsonConvert.DeserializeObject<NewReservationEventData>(@event.Data)!;

        item.Id = @event.StreamId;
        item.Version = @event.Version;

        item.Status = ReservationStatus.New;
        item.HotelDays = data.HotelDays;
        item.RoomsStudio = data.RoomsStudio;
        item.RoomsSmall = data.RoomsSmall;
        item.RoomsMedium = data.RoomsMedium;
        item.RoomsLarge = data.RoomsLarge;
        item.RoomsApartment = data.RoomsApartment;
    }

    private static void ApplyAccepted(this ReservationModel item, BaseEvent @event)
    {
        item.Version = @event.Version;

        item.Status = ReservationStatus.Accepted;
    }

    private static void ApplyRejected(this ReservationModel item, BaseEvent @event)
    {
        item.Version = @event.Version;

        item.Status = ReservationStatus.Rejected;
    }
}
