using System;
using System.ComponentModel.DataAnnotations;
using TripBooker.Common.Transport;

namespace TripBooker.Common.TravelAgency.Model;

public class TransportModel
{
    public int AvailablePlaces { get; set; }

    public string DepartureAirportCode { get; set; } = null!;

    public string DepartureAirportName { get; set; } = null!;

    public DateTime DepartureDate { get; set; }

    public string DestinationAirportCode { get; set; } = null!;

    public string DestinationAirportName { get; set; } = null!;

    [Key]
    public Guid Id { get; set; }

    public int TicketPrice { get; set; }

    public TransportType Type { get; set; }
}