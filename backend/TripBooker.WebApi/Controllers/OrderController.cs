using MassTransit;
using Microsoft.AspNetCore.Mvc;
using TripBooker.Common.Order;
using TripBooker.Common.Payment;

namespace TripBooker.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IBus _bus;

    public OrderController(IBus bus)
    {
        _bus = bus;
    }

    [HttpPost("Submit")]
    public Guid Submit(SubmitOrder submitOrder, CancellationToken cancellationToken)
    {
        // TODO: replace with minimal API contract and map to internal SubmitOrder model

        var guid = Guid.NewGuid();

        submitOrder.Order.OrderId = guid;
        _bus.Publish(submitOrder, cancellationToken);

        return guid;
    }

    [HttpPost("Pay")]
    public void Pay(PaymentCommand paymentCommand, CancellationToken cancellationToken)
    {
        // TODO optionally: replace wih API contract and map to current business internal object
        _bus.Publish(paymentCommand, cancellationToken);
    }
}