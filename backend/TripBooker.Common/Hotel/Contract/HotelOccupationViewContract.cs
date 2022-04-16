using System;

namespace TripBooker.Common.Hotel.Contract;

public class HotelOccupationViewContract
{
    public Guid HotelId { get; set; }
    public string HotelName { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int Rating { get; set; }
    public DateTime Date { get; set; }
    public int RoomsStudio { get; set; }
    public int RoomsSmall { get; set; }
    public int RoomsMedium { get; set; }
    public int RoomsLarge { get; set; }
    public int RoomsApartment { get; set; }
}
