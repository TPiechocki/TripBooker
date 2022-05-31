namespace TripBooker.Common.Statistics;

public class DestinationCountUpdate
{
    public DestinationCountUpdate(string destinationAirportCode, int newCount)
    {
        DestinationAirportCode = destinationAirportCode;
        NewCount = newCount;
    }

    public string DestinationAirportCode { get; }

    public int NewCount { get; }
}