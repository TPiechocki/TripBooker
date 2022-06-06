using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripBooker.Common.TourOperator.Contract.Query;

namespace TripBooker.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class UpdatesController : Controller
{
    private readonly IRequestClient<UpdatesQueryContract> _client;
    private readonly IBus _bus;

    public UpdatesController(
        IRequestClient<UpdatesQueryContract> client, IBus bus)
    {
        _client = client;
        _bus = bus;
    }

    [HttpGet]
    public async Task<UpdatesQueryResultContract> Get(CancellationToken cancellationToken)
    {
        var response = await _client.GetResponse<UpdatesQueryResultContract>(
            new UpdatesQueryContract(), cancellationToken);
        return response.Message;
    }

    [HttpPost]
    public async Task<IActionResult> Post(UpdateSwitchQuery switchUpdate, CancellationToken cancellationToken)
    {
        await _bus.Publish(switchUpdate, cancellationToken);
        return Ok();
    }
}
