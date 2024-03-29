﻿using Newtonsoft.Json;
using TripBooker.Common;
using TripBooker.HotelService.Model.Events.Reservation;

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

                case nameof(ReservationConfirmedEventData):
                    item.ApplyConfirmedEventData(@event);
                    break;

                case nameof(ReservationCancelledEventData):
                    item.ApplyCancelledEventData(@event);
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
        item.MealOption = data.MealOption;
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

    private static void ApplyConfirmedEventData(this ReservationModel item, BaseEvent @event)
    {
        item.Version = @event.Version;

        item.Status = ReservationStatus.Confirmed;
    }

    private static void ApplyCancelledEventData(this ReservationModel item, BaseEvent @event)
    {
        item.Version = @event.Version;

        item.Status = ReservationStatus.Cancelled;
    }
}
