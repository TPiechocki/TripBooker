namespace TripBooker.TransportService.Model.Events.Transport;

public class TransportPlaceUpdateEvent
{
    public TransportPlaceUpdateEvent(
        int newPlaces, 
        int placesDelta, 
        Guid reservationEventId)
    {
        NewPlaces = newPlaces;
        PlacesDelta = placesDelta;
        ReservationEventId = reservationEventId;
    }

    public int NewPlaces { get; }

    public int PlacesDelta { get; }

    public Guid ReservationEventId { get; }
}