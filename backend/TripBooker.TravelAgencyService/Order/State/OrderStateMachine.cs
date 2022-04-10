using MassTransit;
using TripBooker.Common.Order;
using TripBooker.Common.Order.Transport;
using SagaState = MassTransit.State;

namespace TripBooker.TravelAgencyService.Order.State;

internal class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    private readonly ILogger<OrderStateMachine> _logger;

    public OrderStateMachine(ILogger<OrderStateMachine> logger)
    {
        _logger = logger;

        InstanceState(x => x.State);
        ConfigureEventCorrelationIds();

        Initially(SetSubmitOrderHandler());
        During(AwaitingTransportConfirmation, SetAcceptOrderHandler());
    }

    private void ConfigureEventCorrelationIds()
    {
        Event(() => SubmitOrder, x =>
            x.CorrelateById(c => c.Message.Order.OrderId));

        Event(() => AcceptTransport, x =>
            x.CorrelateById(c => c.Message.CorrelationId));
    }

    private EventActivityBinder<OrderState, SubmitOrder> SetSubmitOrderHandler() =>
        When(SubmitOrder)
            .Then(x => UpdateSagaState(x.Saga, x.Message))
            .Then(x => _logger.LogInformation($"New order received (OrderId={x.Message.Order.OrderId})."))
            .TransitionTo(AwaitingTransportConfirmation)
            .ThenAsync(x => x.Publish(new NewTransportReservation
            {
                Order = x.Message.Order
            }));

    private EventActivityBinder<OrderState, TransportReservationAccepted> SetAcceptOrderHandler() =>
        When(AcceptTransport)
            .TransitionTo(TransportAccepted)    // TODO: save reservation id
            .Then(x => _logger.LogInformation($"Transport reservation accepted (OrderId={x.Message.CorrelationId})."))
            .Finalize();
            


    private static void UpdateSagaState(OrderState state, OrderCommand data)
    {
        state.Order = data.Order;
    }


    public SagaState AwaitingTransportConfirmation { get; private set; } = null!;

    public SagaState TransportAccepted { get; private set; } = null!;

    public Event<TransportReservationAccepted> AcceptTransport { get; private set; } = null!;

    public Event<SubmitOrder> SubmitOrder { get; private set; } = null!;
}