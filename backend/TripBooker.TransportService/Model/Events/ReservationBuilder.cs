using Newtonsoft.Json;
using TripBooker.Common;
using TripBooker.Common.Transport;
using TripBooker.TransportService.Model.Events.Reservation;

namespace TripBooker.TransportService.Model.Events;

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
        item.TransportId = data.TransportId;
        item.Places = data.Places;
    }

    private static void ApplyAccepted(this ReservationModel item, BaseEvent @event)
    {
        var data = JsonConvert.DeserializeObject<ReservationAcceptedEventData>(@event.Data)!;

        item.Version = @event.Version;

        item.Status = ReservationStatus.Accepted;
        item.Price = data.Price;
    }

    private static void ApplyRejected(this ReservationModel item, BaseEvent @event)
    {
        item.Version = @event.Version;

        item.Status = ReservationStatus.Rejected;
    }
}