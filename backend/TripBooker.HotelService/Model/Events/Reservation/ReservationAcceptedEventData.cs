namespace TripBooker.HotelService.Model.Events.Reservation;

internal class ReservationAcceptedEventData
{
    public ReservationAcceptedEventData(double price)
    {
        Price = price;
    }

    public double Price { get; set; }
}

