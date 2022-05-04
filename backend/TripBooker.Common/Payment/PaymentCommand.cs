using System;

namespace TripBooker.Common.Payment;

public class PaymentCommand : ContractBase
{
    public PaymentCommand(Guid correlationId) : base(correlationId)
    {
    }
}