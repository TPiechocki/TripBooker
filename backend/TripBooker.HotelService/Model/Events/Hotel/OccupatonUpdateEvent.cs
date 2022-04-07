namespace TripBooker.HotelService.Model.Events.Hotel;

internal class OccupatonUpdateEvent
{
    public Guid ReservationEventId { get; }

    public int RoomsStudio { get; set; }

    public int RoomsSmall { get; set; }

    public int RoomsMedium { get; set; }

    public int RoomsLarge { get; set; }

    public int RoomsApartment { get; set; }
}
