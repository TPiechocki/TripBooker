namespace TripBooker.TransportService.Model.Events.Reservation;

internal class NewReservationEventData
{
    public NewReservationEventData(Guid transportId, int places)
    {
        TransportId = transportId;
        Places = places;
    }

    public Guid TransportId { get; }

    public int Places { get; }
}