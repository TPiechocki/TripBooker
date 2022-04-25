namespace TripBooker.TransportService.Model.Events.Reservation;

internal class ReservationAcceptedEventData
{
    public ReservationAcceptedEventData(int price)
    {
        Price = price;
    }

    public int Price { get; }
}