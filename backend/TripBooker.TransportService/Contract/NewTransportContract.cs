namespace TripBooker.TransportService.Contract;

public class NewTransportContract
{
    public NewTransportContract(DateTime departureDate, bool isReturn, int places, int transportOptionId)
    {
        DepartureDate = departureDate;
        IsReturn = isReturn;
        Places = places;
        TransportOptionId = transportOptionId;
    }

    public DateTime DepartureDate { get; }

    public bool IsReturn { get; }

    public int Places { get; }

    public int TransportOptionId { get; }

}