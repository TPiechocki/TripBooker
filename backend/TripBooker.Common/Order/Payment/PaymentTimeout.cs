using System;

namespace TripBooker.Common.Order.Payment;

public class PaymentTimeout : ContractBase
{
    public PaymentTimeout(
        Guid correlationId)
        : base(correlationId)
    {
    }
}