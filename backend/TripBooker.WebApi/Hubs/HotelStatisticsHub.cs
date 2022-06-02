using MassTransit;
using Microsoft.AspNetCore.SignalR;
using TripBooker.Common.Statistics.Query;
using TripBooker.Common.Statistics.Updates;

namespace TripBooker.WebApi.Hubs;

public interface IHotelStatisticsClient
{
    Task HotelCountUpdate(HotelCount update);

    Task HotelsResponse(GetHotelCountsResponse response);

    Task HotelResponse(HotelCount response);
}

public class HotelStatisticsHub : Hub<IHotelStatisticsClient>
{
    private readonly IRequestClient<GetHotelCount> _hotelRequestClient;
    private readonly IRequestClient<GetHotelCountsQuery> _hotelsRequestClient;

    public HotelStatisticsHub(
        IRequestClient<GetHotelCountsQuery> hotelsRequestClient,
        IRequestClient<GetHotelCount> hotelRequestClient)
    {
        _hotelsRequestClient = hotelsRequestClient;
        _hotelRequestClient = hotelRequestClient;
    }

    public async Task GetForDestination(GetHotelCountsQuery query)
    {
        var counts = await _hotelsRequestClient.GetResponse<GetHotelCountsResponse>(query);

        await Clients.Caller.HotelsResponse(counts.Message);
    }

    public async Task GetForHotel(GetHotelCount query)
    {
        var counts = await _hotelRequestClient.GetResponse<HotelCount>(query);

        await Clients.Caller.HotelResponse(counts.Message);
    }
}