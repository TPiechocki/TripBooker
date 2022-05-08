using System;

namespace TripBooker.Common.Order.Payment;

public class PaymentAccepted : ContractBase
{
    public PaymentAccepted(
        Guid correlationId)
        : base(correlationId)
    {
    }
}