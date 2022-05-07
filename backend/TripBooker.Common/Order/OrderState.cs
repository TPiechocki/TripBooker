using System;
using MassTransit;

namespace TripBooker.Common.Order;

public class OrderState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }

    public string State { get; set; } = null!;

    public OrderData Order { get; set; } = null!;
}
