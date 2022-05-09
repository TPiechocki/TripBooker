using System;

namespace TripBooker.Common.Payment;

public class PaymentCommand : ContractBase
{
    public PaymentCommand(Guid correlationId) : base(correlationId)
    {
    }
}

public class PaymentCommandResponse : ContractBase
{
    public PaymentCommandResponse(Guid correlationId) : base(correlationId)
    {
    }
}