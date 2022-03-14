using TripBooker.Common.Transport;

namespace TripBooker.TransportService.Model.Events;

internal class NewTransportEvent
{
    public int TransportId { get; }

    public DateOnly DepartureDate { get; }

    public string DeparturePlace { get; }

    public string Destination { get; }

    public TransportType Type { get; }

    public int AvailablePlaces { get; }

    public NewTransportEvent(int transportId, DateOnly departureDate, string departurePlace, string destination,
        TransportType type, int availablePlaces)
    {
        TransportId = transportId;
        DepartureDate = departureDate;
        DeparturePlace = departurePlace;
        Destination = destination;
        Type = type;
        AvailablePlaces = availablePlaces;
    }
}