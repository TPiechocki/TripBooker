using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripBooker.Common.Order;
using TripBooker.Common.Payment;

namespace TripBooker.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IBus _bus;
    private readonly IRequestClient<OrderStatus> _orderStatusClient;

    public OrderController(
        IBus bus, 
        IRequestClient<OrderStatus> orderStatusClient)
    {
        _bus = bus;
        _orderStatusClient = orderStatusClient;
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

    [AllowAnonymous]
    [HttpGet("{guid}")]
    [ProducesResponseType(typeof(OrderStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Status(Guid guid, CancellationToken cancellationToken)
    {
        var orderStatus = await _orderStatusClient
            .GetResponse<OrderState>(new OrderStatus(guid), cancellationToken);

        if (orderStatus.Message.CorrelationId == Guid.Empty)
        {
            return NotFound();
        }

        return Ok(new OrderStatusResponse(orderStatus.Message));
    }
}