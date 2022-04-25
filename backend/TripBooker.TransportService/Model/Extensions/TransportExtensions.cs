using TripBooker.TransportService.Contract;
using TripBooker.TransportService.Model.Events.Transport;

namespace TripBooker.TransportService.Model.Extensions;

internal static class TransportExtensions
{
    public static NewTransportEventData MapToNewTransportEventData(this NewTransportContract transport)
    {
        return new NewTransportEventData(transport.TransportOptionId, transport.DepartureDate, transport.Places,
            transport.TicketPrice);
    }
}