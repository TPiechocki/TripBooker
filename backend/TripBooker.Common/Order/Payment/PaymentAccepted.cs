using System;

namespace TripBooker.Common.Order.Payment;

public class PaymentAccepted : ContractBase
{
    public PaymentAccepted(
        Guid correlationId,
        int price,
        Guid orderId)
        : base(correlationId)
    {
        Price = price;
        OrderId = orderId;
    }

    public int Price { get; }

    public Guid OrderId { get; }
}