using MassTransit;
using Microsoft.AspNetCore.SignalR;
using TripBooker.Common.Statistics.Query;
using TripBooker.Common.Statistics.Updates;

namespace TripBooker.WebApi.Hubs;

public interface ITransportStatisticsClient
{
    Task TransportCountsUpdate(TransportCounts update);
}

public class TransportStatisticsHub : Hub<ITransportStatisticsClient>
{
    private readonly IRequestClient<GetTransportCountsQuery> _requestClient;

    public TransportStatisticsHub(
        IRequestClient<GetTransportCountsQuery> requestClient)
    {
        _requestClient = requestClient;
    }

    public async Task GetForDestination(GetTransportCountsQuery query)
    {
        var counts = await _requestClient.GetResponse<TransportCounts>(
            query);

        await Clients.Caller.TransportCountsUpdate(counts.Message);
    }
}