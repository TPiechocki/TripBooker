using System;
using System.Collections.Generic;

namespace TripBooker.Common.TourOperator.Contract;

public class HotelUpdateContract
{
    public Guid HotelId { get; set; }

    public List<Guid> HotelDays { get; set; } = new List<Guid>();

    public double PriceModifierFactor { get; set; } = 1.0;

    public int RoomsStudioChange { get; set; } = 0;

    public int RoomsSmallChange { get; set; } = 0;

    public int RoomsMediumChange { get; set; } = 0;

    public int RoomsLargeChange { get; set; } = 0;

    public int RoomsApartmentChange { get; set; } = 0;
}
