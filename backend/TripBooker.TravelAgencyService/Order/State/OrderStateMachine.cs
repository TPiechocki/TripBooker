using AutoMapper;
using MassTransit;
using TripBooker.Common.Order;
using TripBooker.Common.Order.Transport;
using SagaState = MassTransit.State;

namespace TripBooker.TravelAgencyService.Order.State;

internal class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    private readonly ILogger<OrderStateMachine> _logger;
    private readonly IMapper _mapper;

    public OrderStateMachine(ILogger<OrderStateMachine> logger, IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;

        InstanceState(x => x.State);
        ConfigureEventCorrelationIds();

        Initially(SetSubmitOrderHandler());
        During(AwaitingTransportConfirmation, SetAcceptTransportHandler(), SetRejectTransportHandler());
    }

    private void ConfigureEventCorrelationIds()
    {
        Event(() => SubmitOrder, x =>
            x.CorrelateById(c => c.Message.Order.OrderId));

        Event(() => AcceptTransport, x =>
            x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => RejectTransport, x =>
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

    private EventActivityBinder<OrderState, TransportReservationAccepted> SetAcceptTransportHandler() =>
        When(AcceptTransport)
            .TransitionTo(TransportAccepted)
            .Then(x => x.Saga.Order = _mapper.Map(x.Message, x.Saga.Order))
            .Then(x => _logger.LogInformation($"Transport reservation accepted (OrderId={x.Message.CorrelationId})."))
            .Finalize();

    private EventActivityBinder<OrderState, TransportReservationRejected> SetRejectTransportHandler() =>
        When(RejectTransport)
            .Then(x =>
            {
                x.Saga.Order = _mapper.Map(x.Message, x.Saga.Order);
                x.Saga.Order.FailureMessage = "Reservation was rejected.";
            })
            .Then(x => _logger.LogInformation($"Transport reservation rejected (OrderId={x.Message.CorrelationId})."))
            .Finalize();

    private static void UpdateSagaState(OrderState state, OrderCommand data)
    {
        state.Order = data.Order;
    }

    // SUBMIT - new order
    public Event<SubmitOrder> SubmitOrder { get; private set; } = null!;


    // TRANSPORT RESERVATION
    public SagaState AwaitingTransportConfirmation { get; private set; } = null!;

    public Event<TransportReservationAccepted> AcceptTransport { get; private set; } = null!;

    public Event<TransportReservationRejected> RejectTransport { get; private set; } = null!;

    public SagaState TransportAccepted { get; private set; } = null!;

}