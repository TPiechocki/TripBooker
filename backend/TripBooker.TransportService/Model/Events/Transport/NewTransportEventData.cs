using TripBooker.Common.Transport;

namespace TripBooker.TransportService.Model.Events.Transport;

internal class NewTransportEventData
{
    public DateTime DepartureDate { get; }

    public string DeparturePlace { get; }

    public string Destination { get; }

    public int TransportOptionId { get; }

    public TransportType Type { get; }

    public int AvailablePlaces { get; }

    public NewTransportEventData(DateTime departureDate, 
        string departurePlace, string destination, int transportOptionId,
        TransportType type, int availablePlaces)
    {
        DepartureDate = departureDate;
        DeparturePlace = departurePlace;
        Destination = destination;
        TransportOptionId = transportOptionId;
        Type = type;
        AvailablePlaces = availablePlaces;
    }
}