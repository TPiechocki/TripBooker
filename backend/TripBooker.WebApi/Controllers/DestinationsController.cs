using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripBooker.Common.TravelAgency.Contract.Query;

namespace TripBooker.WebApi.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class DestinationsController : ControllerBase
    {
        private readonly IRequestClient<DestinationsQueryContract> _client;

        public DestinationsController( 
            IRequestClient<DestinationsQueryContract> client)
        {
            _client = client;
        }

        [HttpGet]
        public async Task<DestinationsQueryResultContract> Get(CancellationToken cancellationToken)
        {
            var response = await _client.GetResponse<DestinationsQueryResultContract>(
                new DestinationsQueryContract(), cancellationToken);
            return response.Message;
        }
    }
}