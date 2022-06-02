using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripBooker.Common.TourOperator.Contract.Query;

namespace TripBooker.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class UpdateController : ControllerBase
{

    private readonly IBus _bus;

    public UpdateController(IBus bus)
    {
        _bus = bus;
    }

    [HttpPost("Hotel")]
    public async Task<IActionResult> Hotel(HotelUpdateQuery query, CancellationToken cancellationToken)
    {
        await _bus.Publish(query, cancellationToken);

        return Ok();
    }

    [HttpPost("Transport")]
    public async Task<IActionResult> Transport(TransportUpdateQuery query, CancellationToken cancellationToken)
    {
        await _bus.Publish(query, cancellationToken);

        return Ok();
    }
}
