using System.Collections.Generic;

namespace TripBooker.Common.Statistics.Updates;

public class TransportCounts
{
    public TransportCounts(
        IEnumerable<TransportCount> transports,
        IEnumerable<TransportCount> returnTransports)
    {
        Transports = transports;
        ReturnTransports = returnTransports;
    }

    public IEnumerable<TransportCount> Transports { get; }

    public IEnumerable<TransportCount> ReturnTransports { get; }
}

public class TransportCount
{
    public TransportCount(
        string? destinationAirportCode,
        int count)
    {
        DestinationAirportCode = destinationAirportCode;
        Count = count;
    }

    public string? DestinationAirportCode { get; }

    public int Count { get; }
}