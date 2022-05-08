namespace TripBooker.PaymentService.Model.Events.Payment;

public class NewPaymentEventData
{
    public NewPaymentEventData(
        double price)
    {
        Price = price;
    }

    public double Price { get; set; }
}
