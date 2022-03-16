namespace TripBooker.TransportService.Contract;

public class NewTransportContract
{
    public NewTransportContract(DateOnly departureDate, bool isReturn, int places, int transportOptionId)
    {
        DepartureDate = departureDate;
        IsReturn = isReturn;
        Places = places;
        TransportOptionId = transportOptionId;
    }

    public DateOnly DepartureDate { get; }

    public bool IsReturn { get; }

    public int Places { get; }

    public int TransportOptionId { get; }

}