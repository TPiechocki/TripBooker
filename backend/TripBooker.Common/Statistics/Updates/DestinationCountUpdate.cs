namespace TripBooker.Common.Statistics.Updates;

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