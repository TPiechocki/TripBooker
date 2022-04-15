namespace TripBooker.TransportService.Contract;

public class NewTransportContract
{
    public NewTransportContract(int transportOptionId, DateTime departureDate, int places, int ticketPrice)
    {
        DepartureDate = departureDate;
        Places = places;
        TicketPrice = ticketPrice;
        TransportOptionId = transportOptionId;
    }

    public DateTime DepartureDate { get; }

    public int Places { get; }

    public int TicketPrice { get; }

    public int TransportOptionId { get; }
}
