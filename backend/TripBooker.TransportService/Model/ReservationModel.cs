using TripBooker.Common;
using TripBooker.Common.Transport;

namespace TripBooker.TransportService.Model;

internal class ReservationModel : EventModel
{
    public Guid TransportId { get; set; }

    public int Places { get; set; }

    public ReservationStatus Status { get; set; }
}