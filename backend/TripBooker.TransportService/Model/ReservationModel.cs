using TripBooker.Common;

namespace TripBooker.TransportService.Model;

internal class ReservationModel : EventModel
{
    public Guid TransportId { get; set; }

    public int Places { get; set; }

    public int Price { get; set; }

    public ReservationStatus Status { get; set; }
}