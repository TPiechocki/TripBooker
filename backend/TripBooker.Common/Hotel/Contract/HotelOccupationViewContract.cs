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
    public double BreakfestPrice { get; set; }
    public bool AllInclusive { get; set; }
    public double AllInclusivePrice { get; set; }
    public DateTime Date { get; set; }
    public int RoomsStudio { get; set; }
    public double StudioPrice { get; set; }
    public int RoomsSmall { get; set; }
    public double SmallPrice { get; set; }
    public int RoomsMedium { get; set; }
    public double MediumPrice { get; set; }
    public int RoomsLarge { get; set; }
    public double LargePrice { get; set; }
    public int RoomsApartment { get; set; }
    public double ApartmentPrice { get; set; }
}
