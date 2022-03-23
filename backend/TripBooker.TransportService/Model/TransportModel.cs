using TripBooker.Common;
using TripBooker.Common.Transport;

namespace TripBooker.TransportService.Model;

internal class TransportModel : EventModel
{
    public DateTime DepartureDate { get; set; }

    public string DeparturePlace { get; set; } = null!;

    public string Destination { get; set; } = null!;

    public TransportType Type { get; set; }

    public int AvailablePlaces { get; set; }
}