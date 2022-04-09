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
            departure = transportOption.DestinationAirportCode;
            destination = transportOption.DepartureAirportCode;
        }
        else
        {
            departure = transportOption.DepartureAirportCode;
            destination = transportOption.DestinationAirportCode;
        }

        return new NewTransportEventData(transport.DepartureDate, departure, destination, transport.TransportOptionId,
            transportOption.Type, transport.Places);
    }
}