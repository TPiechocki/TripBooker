using System.ComponentModel.DataAnnotations;
using TripBooker.Common.Transport;

namespace TripBooker.TransportService.Model;

public class TransportView
{
    [Key]
    public int Id { get; set; }

    public DateOnly DepartureDate { get; set; }

    public string DeparturePlace { get; set; } = null!;

    public string Destination { get; set; } = null!;

    public TransportType Type { get; set; }

    public int AvailablePlaces { get; set; }
}