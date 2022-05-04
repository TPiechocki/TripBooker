using System;

namespace TripBooker.Common.Order.Payment;

public class PaymentRejected : ContractBase
{
    public PaymentRejected(
        Guid correlationId)
        : base(correlationId)
    {
    }
}