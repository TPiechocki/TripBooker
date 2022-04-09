namespace TripBooker.TransportService.Contract;

public class NewTransportContract
{
    public NewTransportContract(int transportOptionId, DateTime departureDate, int places)
    {
        DepartureDate = departureDate;
        Places = places;
        TransportOptionId = transportOptionId;
    }

    public DateTime DepartureDate { get; }

    public int Places { get; }

    public int TransportOptionId { get; }

}