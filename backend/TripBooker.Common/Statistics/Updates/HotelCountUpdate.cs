namespace TripBooker.Common.Statistics.Updates;

public class HotelCountUpdate
{
    public HotelCountUpdate(string destination, string hotelCode, int newCount)
    {
        HotelCode = hotelCode;
        NewCount = newCount;
        Destination = destination;
    }

    public string Destination { get; }

    public string HotelCode { get; }

    public int NewCount { get; }
}