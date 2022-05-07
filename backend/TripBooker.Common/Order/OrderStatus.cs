using System;

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
    public OrderStatusResponse(OrderState order)
    {
        Order = order;
    }

    public OrderState Order { get; }
}