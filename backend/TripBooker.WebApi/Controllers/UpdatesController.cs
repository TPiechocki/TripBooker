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

    public UpdatesController(
        IRequestClient<UpdatesQueryContract> client)
    {
        _client = client;
    }

    [HttpGet]
    public async Task<UpdatesQueryResultContract> Get(CancellationToken cancellationToken)
    {
        var response = await _client.GetResponse<UpdatesQueryResultContract>(
            new UpdatesQueryContract(), cancellationToken);
        return response.Message;
    }
}
