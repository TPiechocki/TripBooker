namespace TripBooker.TransportService.Contract;

internal class NewReservationContract
{
    public NewReservationContract(Guid transportId, int places)
    {
        TransportId = transportId;
        Places = places;
    }

    public Guid TransportId { get; }

    public int Places { get; }
}