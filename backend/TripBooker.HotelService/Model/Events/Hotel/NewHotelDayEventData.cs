namespace TripBooker.HotelService.Model.Events.Hotel;

internal class NewHotelDayEventData
{
    public Guid HotelId { get; set; }

    public DateTime Date { get; set; }

    public int RoomsStudio { get; set; }

    public int RoomsSmall { get; set; }

    public int RoomsMedium { get; set; }

    public int RoomsLarge { get; set; }

    public int RoomsApartment { get; set; }
}
