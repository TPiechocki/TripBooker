using System;

namespace TripBooker.Common.Transport.Contract;

public class TransportViewContract
{
    public Guid Id { get; set; }

    public DateTime DepartureDate { get; set; }

    public string DeparturePlace { get; set; } = null!;

    public string Destination { get; set; } = null!;

    public TransportType Type { get; set; }

    public int AvailablePlaces { get; set; }
}