using Microsoft.AspNetCore.SignalR;
using TripBooker.Common.Hubs;

namespace TripBooker.WebApi.Hubs;

public interface IOfferUpdatesClient
{
    public Task HotelUpdatedNotification(HotelViewUpdated updates);
    
    public Task TransportsUpdatedNotification(TransportViewUpdated updates);
}

public class OfferUpdatesHub : Hub<IOfferUpdatesClient>
{
    
}