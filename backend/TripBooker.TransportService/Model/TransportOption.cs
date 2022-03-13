using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TripBooker.Common.Transport;

namespace TripBooker.TransportService.Model;

internal class TransportOption
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string DeparturePlace { get; set; } = null!;

    public string Destination { get; set; } = null!;

    [Required]
    public TransportType Type { get; set; }

    public string Carrier { get; set; } = null!;
}