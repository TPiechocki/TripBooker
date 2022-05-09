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
    private readonly IRequestClient<PaymentStatusQuery> _paymentStatusClient;
    private readonly IRequestClient<PaymentCommand> _paymentCommandClient;

    public OrderController(
        IBus bus, 
        IRequestClient<OrderStatus> orderStatusClient, 
        IRequestClient<PaymentStatusQuery> paymentStatusClient, 
        IRequestClient<PaymentCommand> paymentCommandClient)
    {
        _bus = bus;
        _orderStatusClient = orderStatusClient;
        _paymentStatusClient = paymentStatusClient;
        _paymentCommandClient = paymentCommandClient;
    }

    [HttpPost("Submit")]
    public async Task<Guid> Submit(SubmitOrder submitOrder, CancellationToken cancellationToken)
    {
        // TODO: replace with minimal API contract and map to internal SubmitOrder model

        var guid = Guid.NewGuid();

        submitOrder.Order.OrderId = guid;
        submitOrder.Order.UserName = HttpContext.User.Identity?.Name;
        await _bus.Publish(submitOrder, cancellationToken);

        return guid;
    }

    [HttpPost("Pay/{guid}")]
    public async Task Pay(Guid guid, CancellationToken cancellationToken)
    {
        // TODO optionally: replace wih API contract and map to current business internal object
        await _paymentCommandClient.GetResponse<PaymentCommandResponse>(new PaymentCommand(guid), cancellationToken);
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

        var paymentStatus = await _paymentStatusClient
            .GetResponse<PaymentModel>(new PaymentStatusQuery(guid), cancellationToken);

        var paymentResult = paymentStatus.Message.Id == Guid.Empty
            ? null
            : paymentStatus.Message;

        return Ok(new OrderStatusResponse(orderStatus.Message, paymentResult));
    }
}