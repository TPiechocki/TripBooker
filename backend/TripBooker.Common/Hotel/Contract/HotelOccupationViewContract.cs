﻿using System;

namespace TripBooker.Common.Hotel.Contract;

public class HotelOccupationViewContract
{
    /// <summary>
    /// Id of the hotel day
    /// </summary>
    public Guid Id { get; set; }
    public string AirportCode { get; set; } = null!;
    public bool AllInclusive { get; set; }
    public double AllInclusivePrice { get; set; }
    public double ApartmentPrice { get; set; }
    public double BreakfastPrice { get; set; }
    public string Country { get; set; } = null!;
    public DateTime Date { get; set; }
    public string HotelCode { get; set; } = null!;
    public Guid HotelId { get; set; }
    public string HotelName { get; set; } = null!;
    public double PriceModifier { get; set; } = 1.0;
    public double LargePrice { get; set; }
    public double MediumPrice { get; set; }
    public float Rating { get; set; }
    public int RoomsApartment { get; set; }
    public int RoomsLarge { get; set; }
    public int RoomsMedium { get; set; }
    public int RoomsSmall { get; set; }
    public int RoomsStudio { get; set; }
    public double SmallPrice { get; set; }
    public double StudioPrice { get; set; }
}
