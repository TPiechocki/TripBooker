namespace TripBooker.TransportService.Model.Events.Transport;

public class TicketPriceUpdateEvent
{
    public TicketPriceUpdateEvent(int newPrice)
    {
        NewPrice = newPrice;
    }

    public int NewPrice { get; }
}
