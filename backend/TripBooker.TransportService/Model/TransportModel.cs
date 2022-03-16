using TripBooker.Common.Transport;

namespace TripBooker.TransportService.Model;

public class TransportModel
{
    public Guid Id { get; set; }

    public DateOnly DepartureDate { get; set; }

    public string DeparturePlace { get; set; } = null!;

    public string Destination { get; set; } = null!;

    public TransportType Type { get; set; }

    public int AvailablePlaces { get; set; }

    public int Version { get; set; }
}