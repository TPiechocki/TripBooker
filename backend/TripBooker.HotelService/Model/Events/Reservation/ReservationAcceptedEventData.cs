namespace TripBooker.HotelService.Model.Events;

internal class ReservationAcceptedEventData
{
    public ReservationAcceptedEventData(double price)
    {
        Price = price;
    }

    public double Price { get; set; }
}

