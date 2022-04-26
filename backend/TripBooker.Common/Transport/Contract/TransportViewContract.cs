using System;
using System.Collections.Generic;

namespace TripBooker.Common.Transport.Contract;

public class TransportViewContract
{
    public int AvailablePlaces { get; set; }
    public string DepartureAirportCode { get; set; } = null!;
    public string DepartureAirportName { get; set; } = null!;
    public DateTime DepartureDate { get; set; }
    public string DestinationAirportCode { get; set; } = null!;
    public string DestinationAirportName { get; set; } = null!;
    public Guid Id { get; set; }
    public int TicketPrice { get; set; }
    public TransportType Type { get; set; }
}

public class ManyTransportsViewContract
{
    public ManyTransportsViewContract(IEnumerable<TransportViewContract> transports)
    {
        Transports = transports;
    }

    public IEnumerable<TransportViewContract> Transports { get; }
}