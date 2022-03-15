namespace TripBooker.TransportService.Model.Extensions;

internal static class TransportExtensions
{
    public static TransportView MapToTransportView(this Transport transport, TransportOption transportOption)
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

        return new TransportView
        {
            Id = transport.Id,
            DeparturePlace = departure,
            Destination = destination,
            DepartureDate = transport.DepartureDate,
            AvailablePlaces = transport.NumberOfSeats,
            Type = transportOption.Type
        };
    }
}