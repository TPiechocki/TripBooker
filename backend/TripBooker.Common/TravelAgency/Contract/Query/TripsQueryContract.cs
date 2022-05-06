using System;
using System.Collections.Generic;

namespace TripBooker.Common.TravelAgency.Contract.Query;

public class TripsQueryContract
{
    public string AirportCode { get; set; } = null!;

    /// <summary>
    /// Possibly null when when user will travel to hotel on his own.
    /// </summary>
    public string? DepartureAirportCode { get; set; }

    public DateTime DepartureDate { get; set; }

    public int NumberOfDays { get; set; }

    public int NumberOfAdults { get; set; }

    public int NumberOfChildrenUpTo18 { get; set; }

    public int NumberOfChildrenUpTo10 { get; set; }

    public int NumberOfChildrenUpTo3 { get; set; }
}

public class TripsQueryResult
{
    public TripsQueryResult(IEnumerable<TripDescription> trips)
    {
        Trips = trips;
    }

    public IEnumerable<TripDescription> Trips { get; }
}

public class TripDescription
{
    public TripDescription(string hotelCode, string hotelName, double minimalPrice)
    {
        HotelCode = hotelCode;
        HotelName = hotelName;
        MinimalPrice = minimalPrice;
    }

    public string HotelCode { get; }

    public string HotelName { get; }

    public double MinimalPrice { get; }
}