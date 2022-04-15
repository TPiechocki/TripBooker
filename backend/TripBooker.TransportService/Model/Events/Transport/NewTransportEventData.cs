namespace TripBooker.TransportService.Model.Events.Transport;

internal class NewTransportEventData
{
    public NewTransportEventData(
        int transportOptionId,
        DateTime departureDate, 
        int availablePlaces, 
        int ticketPrice)
    {
        DepartureDate = departureDate;
        TransportOptionId = transportOptionId;
        AvailablePlaces = availablePlaces;
        TicketPrice = ticketPrice;
    }

    public int AvailablePlaces { get; }

    public DateTime DepartureDate { get; }

    public int TicketPrice { get; }

    public int TransportOptionId { get; }
}