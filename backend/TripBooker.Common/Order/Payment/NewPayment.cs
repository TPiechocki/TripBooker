using System;

namespace TripBooker.Common.Order.Payment;

public class NewPayment : ContractBase
{
    // CorrelationId represents order id
    public NewPayment(Guid correlationId, double price) 
        : base(correlationId)
    {
        Price = price;
    }

    public double Price { get; }

    // TODO: optionally add user id
}