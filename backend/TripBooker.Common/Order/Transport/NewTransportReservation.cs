namespace TripBooker.Common.Order.Transport;

public class NewTransportReservation : OrderCommand
{
    public bool IsReturn { get; set; }
}