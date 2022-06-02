using AutoMapper;
using MassTransit;
using TripBooker.Common.Hubs;
using TripBooker.Common.Order;
using TripBooker.Common.Order.Hotel;
using TripBooker.Common.Order.Payment;
using TripBooker.Common.Order.Transport;
using TripBooker.Common.Statistics;
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
        During(AwaitingReturnTransportConfirmation, SetAcceptReturnTransportHandler(),
            SetRejectReturnTransportHandler());
        During(AwaitingHotelConfirmation, SetAcceptHotelHandler(), SetRejectHotelHandler());
        During(AwaitingPaymentConfirmation, SetAcceptPaymentHandler(), SetRejectPaymentHandler());

        DuringAny(
            When(OrderStatus)
                .ThenAsync(x =>
                    x.RespondAsync(x.Saga))
        );
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

        Event(() => OrderStatus, x =>
        {
            x.CorrelateById(c => c.Message.OrderId);
            x.ReadOnly = true;

            x.OnMissingInstance(m =>
            {
                return m.ExecuteAsync(e => e.RespondAsync(new OrderState
                {
                    CorrelationId = Guid.Empty
                }));
            });
        });
    }

    private EventActivityBinder<OrderState, SubmitOrder> SetSubmitOrderHandler() =>
        When(SubmitOrder)
            .Then(x => UpdateSagaState(x.Saga, x.Message))
            .Then(x => _logger.LogInformation($"New order received (OrderId={x.Message.Order.OrderId})."))
            .TransitionTo(AwaitingTransportConfirmation)
            .IfElse(x => x.Saga.Order.TransportId != null,
                x =>
                    x.ThenAsync(a => a.Publish(new NewTransportReservation
                    {
                        Order = a.Saga.Order,
                        IsReturn = false
                    })),
                x => x.ThenAsync(a =>
                    a.Publish(new TransportReservationAccepted
                    (
                        a.Saga.CorrelationId, 0, Guid.Empty, null
                    ))));

    private EventActivityBinder<OrderState, TransportReservationAccepted> SetAcceptTransportHandler() =>
        When(AcceptTransport)
            .TransitionTo(AwaitingReturnTransportConfirmation)
            .Then(x =>
            {
                x.Saga.Order.TransportPrice = x.Message.Price;
                x.Saga.Order.TransportReservationId = x.Message.ReservationId;
                x.Saga.Order.DepartureAirportCode = x.Message.LocalAirportCode;
            })
            .Then(x => _logger.LogInformation($"Transport reservation accepted (OrderId={x.Message.CorrelationId})."))
            .IfElse(x => x.Saga.Order.ReturnTransportId != null,
                x =>
                    x.ThenAsync(a => a.Publish(new NewTransportReservation
                    {
                        Order = a.Saga.Order,
                        IsReturn = true
                    })),
                x => x.ThenAsync(a =>
                    a.Publish(new TransportReservationAccepted
                    (
                        a.Saga.CorrelationId, 0, Guid.Empty, null
                    ))));

    private EventActivityBinder<OrderState, TransportReservationRejected> SetRejectTransportHandler() =>
        When(RejectTransport)
            .Then(x =>
            {
                x.Saga.Order.TransportReservationId = x.Message.ReservationId;
                x.Saga.Order.FailureMessage = "Transport reservation was rejected.";
            })
            .Then(x => _logger.LogInformation($"Transport reservation rejected (OrderId={x.Message.CorrelationId})."))
            .TransitionTo(Rejected);


    private EventActivityBinder<OrderState, TransportReservationAccepted> SetAcceptReturnTransportHandler() =>
        When(AcceptTransport)
            .TransitionTo(AwaitingHotelConfirmation)
            .Then(x =>
            {
                x.Saga.Order.ReturnTransportPrice = x.Message.Price;
                x.Saga.Order.ReturnTransportReservationId = x.Message.ReservationId;
                x.Saga.Order.ReturnAirportCode = x.Message.LocalAirportCode;
            })
            .Then(x => _logger.LogInformation(
                $"Return transport reservation accepted (OrderId={x.Message.CorrelationId})."))
            .ThenAsync(x => x.Publish(new NewHotelReservation
                {
                    Order = x.Saga.Order
                }
            ));

    private EventActivityBinder<OrderState, TransportReservationRejected> SetRejectReturnTransportHandler() =>
        When(RejectTransport)
            .Then(x =>
            {
                x.Saga.Order.ReturnTransportReservationId = x.Message.ReservationId;
                x.Saga.Order.FailureMessage = "Return transport reservation was rejected.";
            })
            .Then(x => _logger.LogInformation(
                $"Return transport reservation rejected (OrderId={x.Message.CorrelationId})."))
            .If(x => x.Saga.Order.TransportId != null,
                a =>
                    a.ThenAsync(x =>
                        x.Publish(new CancelTransportReservation
                        (
                            x.Saga.CorrelationId,
                            x.Saga.Order.TransportReservationId!.Value
                        ))))
            .TransitionTo(Rejected);


    private EventActivityBinder<OrderState, HotelReservationAccepted> SetAcceptHotelHandler() =>
        When(AcceptHotel)
            .TransitionTo(AwaitingPaymentConfirmation)
            .Then(x =>
            {
                x.Saga.Order.HotelPrice = x.Message.Price;
                x.Saga.Order.HotelReservationId = x.Message.ReservationId;
                x.Saga.Order.DestinationAirportCode = x.Message.DestinationAirportCode;
            })
            .Then(x => _logger.LogInformation(
                $"Hotel reservation accepted (OrderId={x.Message.CorrelationId})."))
            .ThenAsync(x => x.Publish(new NewPayment
                (
                    x.Saga.CorrelationId, x.Saga.Order.Price, x.Saga.Order.DiscountCode
                )
            ))
            .ThenAsync(x => x.Publish(
                _mapper.Map<NewReservationEvent>(x.Saga.Order)));

    private EventActivityBinder<OrderState, HotelReservationRejected> SetRejectHotelHandler() =>
        When(RejectHotel)
            .Then(x =>
            {
                x.Saga.Order.HotelReservationId = x.Message.ReservationId;
                x.Saga.Order.FailureMessage = "Hotel reservation was rejected.";
            })
            .Then(x => _logger.LogInformation(
                $"Hotel reservation rejected (OrderId={x.Message.CorrelationId})."))
            .If(x => x.Saga.Order.TransportId != null,
                a =>
                    a.ThenAsync(x =>
                        x.Publish(new CancelTransportReservation
                        (
                            x.Saga.CorrelationId,
                            x.Saga.Order.TransportReservationId!.Value
                        ))))
            .If(x => x.Saga.Order.ReturnTransportId != null,
                a =>
                    a.ThenAsync(x =>
                        x.Publish(new CancelTransportReservation
                        (
                            x.Saga.CorrelationId,
                            x.Saga.Order.ReturnTransportReservationId!.Value
                        ))))
            .TransitionTo(Rejected);

    
    
    private EventActivityBinder<OrderState, PaymentAccepted> SetAcceptPaymentHandler() =>
        When(AcceptPayment)
            .Then(x => _logger.LogInformation($"Payment accepted (OrderId={x.Message.CorrelationId})."))
            .If(x => x.Saga.Order.TransportId != null,
                a =>
                    a.ThenAsync(x =>
                        x.Publish(new ConfirmTransportReservation
                        (
                            x.Saga.CorrelationId,
                            x.Saga.Order.TransportReservationId!.Value
                        ))))
            .If(x => x.Saga.Order.ReturnTransportId != null,
                a =>
                    a.ThenAsync(x =>
                        x.Publish(new ConfirmTransportReservation
                        (
                            x.Saga.CorrelationId,
                            x.Saga.Order.ReturnTransportReservationId!.Value
                        ))))
            .ThenAsync(x =>
                x.Publish(new ConfirmHotelReservation(
                    x.Saga.CorrelationId,
                    x.Saga.Order.HotelReservationId!.Value
                )))
            .ThenAsync(x =>
                x.Publish(new TourOperatorReport
                {
                    Order = x.Saga.Order
                }))
            .ThenAsync(x =>
                x.Publish(new PurchasedOfferNotification
                {
                    OrderId = x.Saga.CorrelationId,
                    PurchasedHotelDays = x.Saga.Order.HotelDays
                }))
            .TransitionTo(Confirmed);

    private EventActivityBinder<OrderState, PaymentTimeout> SetRejectPaymentHandler() =>
        When(RejectPayment)
            .Then(x =>
            {
                x.Saga.Order.FailureMessage = "Payment timed out.";
            })
            .Then(x => _logger.LogInformation(
                $"Payment timed out (OrderId={x.Message.CorrelationId})."))
            // TODO optionally: cancel differently for rejected payment to get different status of reservation
            .If(x => x.Saga.Order.TransportId != null,
                a =>
                    a.ThenAsync(x =>
                        x.Publish(new CancelTransportReservation
                        (
                            x.Saga.CorrelationId,
                            x.Saga.Order.TransportReservationId!.Value
                        ))))
            .If(x => x.Saga.Order.ReturnTransportId != null,
                a =>
                    a.ThenAsync(x =>
                        x.Publish(new CancelTransportReservation
                        (
                            x.Saga.CorrelationId,
                            x.Saga.Order.ReturnTransportReservationId!.Value
                        ))))
            .ThenAsync(x =>
                x.Publish(new CancelHotelReservation(
                    x.Saga.CorrelationId,
                    x.Saga.Order.HotelReservationId!.Value
                )))
            .TransitionTo(Rejected);


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

    public SagaState AwaitingReturnTransportConfirmation { get; private set; } = null!;


    public SagaState AwaitingHotelConfirmation { get; private set; } = null!;

    public Event<HotelReservationAccepted> AcceptHotel { get; private set; } = null!;

    public Event<HotelReservationRejected> RejectHotel { get; private set; } = null!;


    public SagaState AwaitingPaymentConfirmation { get; private set; } = null!;

    public Event<PaymentAccepted> AcceptPayment { get; private set; } = null!;

    public Event<PaymentTimeout> RejectPayment { get; private set; } = null!;


    public SagaState Confirmed { get; private set; } = null!;

    public SagaState Rejected { get; private set; } = null!;


    public Event<OrderStatus> OrderStatus { get; private set; } = null!;

}