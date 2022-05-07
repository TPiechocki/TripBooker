using TripBooker.Common;
using TripBooker.Common.Payment;

namespace TripBooker.PaymentService.Model;

internal class PaymentModel : EventModel
{
    public double Price { get; set; }

    public PaymentStatus Status { get; set; }
}
