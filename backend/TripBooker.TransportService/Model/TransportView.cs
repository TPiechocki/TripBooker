using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TripBooker.Common.Transport;

namespace TripBooker.TransportService.Model;

public class TransportView
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int TransportId { get; set; }

    public DateOnly DepartureDate { get; set; }

    public string DeparturePlace { get; set; } = null!;

    public string Destination { get; set; } = null!;

    public TransportType Type { get; set; }

    public int AvailablePlaces { get; set; }
}