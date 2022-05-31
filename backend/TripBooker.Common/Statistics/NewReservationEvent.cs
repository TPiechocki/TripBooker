using System;

namespace TripBooker.Common.Statistics;

public class NewReservationEvent
{
    public Guid OrderId { get; set; }

    public string DestinationAirportCode { get; set; } = null!;

    public string HotelCode { get; set; } = null!;

    public string? DepartureAirportCode { get; set; }

    public string? ReturnAirportCode { get; set; }
}