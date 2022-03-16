using TripBooker.TransportService.Contract;
using TripBooker.TransportService.Model.Events.Transport;

namespace TripBooker.TransportService.Model.Extensions;

internal static class TransportExtensions
{
    public static NewTransportEventData MapToNewTransportEventData(this NewTransportContract transport, TransportOption transportOption)
    {
        string destination;
        string departure;
        if (transport.IsReturn)
        {
            departure = transportOption.Destination;
            destination = transportOption.DeparturePlace;
        }
        else
        {
            departure = transportOption.DeparturePlace;
            destination = transportOption.Destination;
        }

        return new NewTransportEventData(transport.DepartureDate, departure, destination, transport.TransportOptionId,
            transportOption.Type, transport.Places);
    }
}