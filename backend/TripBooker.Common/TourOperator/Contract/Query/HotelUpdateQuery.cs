using System;

namespace TripBooker.Common.TourOperator.Contract.Query;

public class HotelUpdateQuery
{
    public Guid HotelId { get; set; }

    public DateTime StartDate { get; set; }

    public int Length { get; set; }

    public double PriceModifierFactor { get; set; } = 1.0;

    public int RoomsStudioChange { get; set; } = 0;

    public int RoomsSmallChange { get; set; } = 0;

    public int RoomsMediumChange { get; set; } = 0;

    public int RoomsLargeChange { get; set; } = 0;

    public int RoomsApartmentChange { get; set; } = 0;
}
