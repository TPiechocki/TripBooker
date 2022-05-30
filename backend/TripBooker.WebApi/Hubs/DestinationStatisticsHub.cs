using Microsoft.AspNetCore.SignalR;
using TripBooker.Common.Statistics;

namespace TripBooker.WebApi.Hubs;

public interface IDestinationStatisticsClient
{
    Task DestinationCountUpdate(DestinationCountUpdate update);
}

public class DestinationStatisticsHub : Hub<IDestinationStatisticsClient>
{
}