using MassTransit;
using MassTransit.Clients;
using Microsoft.AspNetCore.Mvc;
using TripBooker.Common.TravelAgency.Contract.Query;

namespace TripBooker.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
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