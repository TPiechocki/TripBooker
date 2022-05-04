using System;

namespace TripBooker.Common.Order.Payment;

public class PaymentRejected : ContractBase
{
    public PaymentRejected(
        Guid correlationId,
        Guid? orderId)
        : base(correlationId)
    {
        OrderId = orderId;
    }

    /// <summary>
    /// Id of the rejected payment.
    /// Can be null for unknown and unhandled errors when only correlation id is known.
    /// </summary>
    public Guid? OrderId { get; }
}