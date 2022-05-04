using System;

namespace TripBooker.Common.Order.Payment;

public class PaymentAccepted : ContractBase
{
    public PaymentAccepted(
        Guid correlationId,
        int price)
        : base(correlationId)
    {
        Price = price;
    }

    public int Price { get; }
}