using MassTransit;
using Microsoft.AspNetCore.SignalR;
using TripBooker.Common.Statistics.Query;
using TripBooker.Common.Statistics.Updates;

namespace TripBooker.WebApi.Hubs;

public interface IDestinationStatisticsClient
{
    Task DestinationCountUpdate(DestinationCountUpdate update);

    Task DestinationCountsResponse(GetDestinationCountsResponse update);
}

public class DestinationStatisticsHub : Hub<IDestinationStatisticsClient>
{
    private readonly IRequestClient<GetDestinationCountsQuery> _requestClient;

    public DestinationStatisticsHub(
        IRequestClient<GetDestinationCountsQuery> requestClient)
    {
        _requestClient = requestClient;
    }

    public async Task GetAll()
    {
        var counts = await _requestClient.GetResponse<GetDestinationCountsResponse>(
            new GetDestinationCountsQuery());

        await Clients.Caller.DestinationCountsResponse(counts.Message);
    }
}