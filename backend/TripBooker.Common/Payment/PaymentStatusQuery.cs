using System;

namespace TripBooker.Common.Payment;

public class PaymentStatusQuery : ContractBase
{
    public PaymentStatusQuery(Guid correlationId) : base(correlationId)
    {
    }
}