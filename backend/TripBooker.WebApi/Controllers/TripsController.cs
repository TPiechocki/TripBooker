using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripBooker.Common.TravelAgency.Contract.Query;

namespace TripBooker.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class TripsController : ControllerBase
{

    private readonly IRequestClient<TripsQueryContract> _tripsClient;

    // TODO: possibly replace with front-end contracts
    public TripsController(IRequestClient<TripsQueryContract> tripsClient)
    {
        _tripsClient = tripsClient;
    }

    [HttpPost]
    public async Task<TripsQueryResult> Post(TripsQueryContract query, CancellationToken cancellationToken)
    {
        var result = await _tripsClient.GetResponse<TripsQueryResult>(query, cancellationToken);
        return result.Message;
    }
}