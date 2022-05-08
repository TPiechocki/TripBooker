using System;

namespace TripBooker.Common.Order.Payment;

public class NewPayment : ContractBase
{
    // CorrelationId represents order id
    public NewPayment(
        Guid correlationId, 
        double price, 
        string? discountCode) 
        : base(correlationId)
    {
        Price = price;
        DiscountCode = discountCode;
    }

    public double Price { get; }

    public string? DiscountCode { get; }

    // TODO: optionally add user id
}