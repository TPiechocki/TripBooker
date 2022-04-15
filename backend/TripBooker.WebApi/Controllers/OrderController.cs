using MassTransit;
using Microsoft.AspNetCore.Mvc;
using TripBooker.Common.Order;

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
    public Guid Submit(SubmitOrder submitOrder)
    {
        // TODO: replace with minimal API contract and map to internal SubmitOrder model

        var guid = Guid.NewGuid();

        submitOrder.Order.OrderId = guid;
        _bus.Publish(submitOrder);

        return guid;
    }
}