using Microsoft.AspNetCore.SignalR;
using TripBooker.Common.Hubs;

namespace TripBooker.WebApi.Hubs;

public interface IPurchasedOfferNotificationClient
{
    Task SendNotification(PurchasedOfferNotification notification);
}

public class PurchasedOfferNotificationHub : Hub<IPurchasedOfferNotificationClient>
{
}