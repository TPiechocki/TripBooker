using System;
using TripBooker.Common.Payment;

namespace TripBooker.Common.Order;

public class OrderStatus
{
    public OrderStatus(Guid orderId)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; }
}

public class OrderStatusResponse
{
    public OrderStatusResponse(
        OrderState order, 
        PaymentModel? payment)
    {
        Order = order;
        Payment = payment;
    }

    public OrderState Order { get; }

    public PaymentModel? Payment { get; }
}