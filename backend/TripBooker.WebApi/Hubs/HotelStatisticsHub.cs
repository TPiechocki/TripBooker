using MassTransit;
using Microsoft.AspNetCore.SignalR;
using TripBooker.Common.Statistics.Query;
using TripBooker.Common.Statistics.Updates;

namespace TripBooker.WebApi.Hubs;

public interface IHotelStatisticsClient
{
    Task HotelCountUpdate(HotelCountUpdate update);

    Task HotelCountsResponse(GetHotelCountsResponse update);
}

public class HotelStatisticsHub : Hub<IHotelStatisticsClient>
{
    private readonly IRequestClient<GetHotelCountsQuery> _requestClient;

    public HotelStatisticsHub(
        IRequestClient<GetHotelCountsQuery> requestClient)
    {
        _requestClient = requestClient;
    }

    public async Task GetForDestination(GetHotelCountsQuery query)
    {
        var counts = await _requestClient.GetResponse<GetHotelCountsResponse>(
            query);

        await Clients.Caller.HotelCountsResponse(counts.Message);
    }
}