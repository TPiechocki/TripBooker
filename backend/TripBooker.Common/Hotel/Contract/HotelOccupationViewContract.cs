using System;

namespace TripBooker.Common.Hotel.Contract;

public class HotelOccupationViewContract
{
    public Guid HotelId { get; set; }
    public string HotelCode { get; set; } = null!;
    public string HotelName { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string AirportCode { get; set; } = null!;
    public float Rating { get; set; }
    public bool AllInclusive { get; set; }
    public DateTime Date { get; set; }
    public int RoomsStudio { get; set; }
    public int RoomsSmall { get; set; }
    public int RoomsMedium { get; set; }
    public int RoomsLarge { get; set; }
    public int RoomsApartment { get; set; }
}
