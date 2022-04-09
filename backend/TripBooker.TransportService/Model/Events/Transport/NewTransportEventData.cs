namespace TripBooker.TransportService.Model.Events.Transport;

internal class NewTransportEventData
{
    public NewTransportEventData(
        int transportOptionId,
        DateTime departureDate, 
        int availablePlaces)
    {
        DepartureDate = departureDate;
        TransportOptionId = transportOptionId;
        AvailablePlaces = availablePlaces;
    }

    public int AvailablePlaces { get; }

    public DateTime DepartureDate { get; }

    public int TransportOptionId { get; }
}