using MassTransit;
using TripBooker.Common.Order;

namespace TripBooker.TravelAgencyService.Order.State;

internal class OrderState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }

    public string State { get; set; } = null!;

    public OrderData Order { get; set; } = null!;
}
