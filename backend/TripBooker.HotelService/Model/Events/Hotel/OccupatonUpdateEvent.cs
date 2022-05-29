namespace TripBooker.HotelService.Model.Events.Hotel;

internal class OccupatonUpdateEvent
{
    public Guid ReservationEventId { get; set; }

    public double PriceModifierFactor { get; set; } = 1.0;

    public int RoomsStudio { get; set; } = 0;

    public int RoomsSmall { get; set; } = 0;

    public int RoomsMedium { get; set; } = 0;

    public int RoomsLarge { get; set; } = 0;

    public int RoomsApartment { get; set; } = 0;
}
