using MassTransit;
using Microsoft.AspNetCore.Mvc;
using TripBooker.Common.TravelAgency.Contract.Query;

namespace TripBooker.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TripController : ControllerBase
{
    private readonly IRequestClient<TripOptionsQueryContract> _tripOptionsClient;
    private readonly IRequestClient<TripQueryContract> _tripClient;

    // TODO: possibly replace with front-end contracts
    public TripController(
        IRequestClient<TripOptionsQueryContract> tripOptionsClient, 
        IRequestClient<TripQueryContract> tripClient)
    {
        _tripOptionsClient = tripOptionsClient;
        _tripClient = tripClient;
    }

    [HttpPost]
    public async Task<TripQueryResponse> Post(TripQueryContract query, CancellationToken cancellationToken)
    {
        var result = await _tripClient.GetResponse<TripQueryResponse>(query, cancellationToken);
        return result.Message;
    }

    [HttpPost("Options")]
    public async Task<TripOptionsQueryResponse> Options(TripOptionsQueryContract query, CancellationToken cancellationToken)
    {
        var result = await _tripOptionsClient.GetResponse<TripOptionsQueryResponse>(query, cancellationToken);
        return result.Message;
    }
}