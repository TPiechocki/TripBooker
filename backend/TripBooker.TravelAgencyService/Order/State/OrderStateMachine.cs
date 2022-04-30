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
        During(AwaitingReturnTransportConfirmation, SetAcceptReturnTransportHandler(), SetRejectReturnTransportHandler());
        During(AwaitingPaymentConfirmation, SetAcceptPaymentHandler(), SetRejectPaymentHandler());
    }

    private void ConfigureEventCorrelationIds()
    {
        Event(() => SubmitOrder, x =>
            x.CorrelateById(c => c.Message.Order.OrderId));

        Event(() => AcceptTransport, x =>
            x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => RejectTransport, x =>
            x.CorrelateById(c => c.Message.CorrelationId));

        Event(() => AcceptPayment, x =>
            x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => RejectPayment, x =>
            x.CorrelateById(c => c.Message.CorrelationId));
    }

    private EventActivityBinder<OrderState, SubmitOrder> SetSubmitOrderHandler() =>
        When(SubmitOrder)
            .Then(x => UpdateSagaState(x.Saga, x.Message))
            .Then(x => _logger.LogInformation($"New order received (OrderId={x.Message.Order.OrderId})."))
            .TransitionTo(AwaitingTransportConfirmation)
            .ThenAsync(x => x.Publish(new NewTransportReservation
            {
                Order = x.Saga.Order,
                IsReturn = false
            }));

    private EventActivityBinder<OrderState, TransportReservationAccepted> SetAcceptTransportHandler() =>
        When(AcceptTransport)
            .TransitionTo(AwaitingReturnTransportConfirmation)
            .Then(x =>
            {
                x.Saga.Order.TransportPrice = x.Message.Price;
                x.Saga.Order.TransportReservationId = x.Message.ReservationId;
            })
            .Then(x => _logger.LogInformation($"Transport reservation accepted (OrderId={x.Message.CorrelationId})."))
            .ThenAsync(x => x.Publish(new NewTransportReservation
            {
                Order = x.Saga.Order,
                IsReturn = true
            }));

    private EventActivityBinder<OrderState, TransportReservationRejected> SetRejectTransportHandler() =>
        When(RejectTransport)
            .Then(x =>
            {
                x.Saga.Order.TransportReservationId = x.Message.ReservationId;
                x.Saga.Order.FailureMessage = "Transport reservation was rejected.";
            })
            .Then(x => _logger.LogInformation($"Transport reservation rejected (OrderId={x.Message.CorrelationId})."))
            .Finalize();

    private EventActivityBinder<OrderState, TransportReservationAccepted> SetAcceptReturnTransportHandler() =>
        When(AcceptTransport)
            .TransitionTo(AwaitingPaymentConfirmation)
            .Then(x =>
            {
                x.Saga.Order.ReturnTransportPrice = x.Message.Price;
                x.Saga.Order.ReturnTransportReservationId = x.Message.ReservationId;
            })
            .Then(x => _logger.LogInformation($"Return transport reservation accepted (OrderId={x.Message.CorrelationId})."))
            .ThenAsync(x => x.Publish(new Payment
            {
                OrderId = x.Saga.Order.OrderId,
                Price = x.Saga.Order.Price
            }));;

    private EventActivityBinder<OrderState, TransportReservationRejected> SetRejectReturnTransportHandler() =>
        When(RejectTransport)
            .Then(x =>
            {
                x.Saga.Order.ReturnTransportReservationId = x.Message.ReservationId;
                x.Saga.Order.FailureMessage = "Return transport reservation was rejected.";
            })
            .Then(x => _logger.LogInformation(
                $"Return transport reservation rejected (OrderId={x.Message.CorrelationId})."))
            .ThenAsync(x =>
                x.Publish(new CancelTransportReservation(x.Saga.CorrelationId,
                    x.Saga.Order.TransportReservationId!.Value)))
            .Finalize();

    private EventActivityBinder<OrderState, TransportReservationAccepted> SetAcceptPaymentHandler() =>
        When(AcceptPayment)
            .Then(x =>
            {
                x.Saga.Order.PaymentId = x.Message.PaymentId;
            })
            .Then(x => _logger.LogInformation($"Payment accepted (OrderId={x.Message.CorrelationId})."))
            .Finalize();

    private EventActivityBinder<OrderState, PaymentRejected> SetRejectPaymentHandler() =>
        When(RejectPayment)
            .Then(x =>
            {
                x.Saga.Order.PaymentId = x.Message.PaymentId;
                x.Saga.Order.FailureMessage = "Payment was rejected.";
            })
            .Then(x => _logger.LogInformation(
                $"Payment rejected (OrderId={x.Message.CorrelationId})."))
            .ThenAsync(x =>
                x.Publish(new CancelTransportReservation(x.Saga.CorrelationId,
                    x.Saga.Order.PaymentId!.Value)))
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

    public Event<PaymentAccepted> AcceptPayment { get; private set; } = null!;

    public Event<PaymentRejected> RejectPayment { get; private set; } = null!;

    public SagaState AwaitingReturnTransportConfirmation { get; private set; } = null!;

    public SagaState AwaitingPaymentConfirmation { get; private set; } = null!;
}