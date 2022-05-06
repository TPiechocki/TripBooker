using System;
using System.Collections.Generic;
using TripBooker.Common.Hotel;

namespace TripBooker.Common.TravelAgency.Contract.Query;

public class TripQueryContract
{
    public IEnumerable<Guid> HotelDays { get; set; } = null!;

    public IEnumerable<Guid> Flights { get; set; } = null!;

    public int NumberOfAdults { get; set; }

    public int NumberOfChildrenUpTo18 { get; set; }

    public int NumberOfChildrenUpTo10 { get; set; }

    public int NumberOfChildrenUpTo3 { get; set; }

    public int NumberOfSmallRooms { get; set; }

    public int NumberOfMediumRooms { get; set; }

    public int NumberOfLargeRooms { get; set; }

    public int NumberOfApartments { get; set; }

    public int NumberOfStudios { get; set; }

    public MealOption MealOption { get; set; }
}

public class TripQueryResponse
{
    public string? ValidationError { get; set; }

    public bool IsAvailable { get; set; }

    public double FinalPrice { get; set; }
}