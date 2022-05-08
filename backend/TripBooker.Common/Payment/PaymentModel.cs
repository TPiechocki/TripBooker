namespace TripBooker.Common.Payment;

public class PaymentModel : EventModel
{
    public double Price { get; set; }

    public PaymentStatus Status { get; set; }
}
