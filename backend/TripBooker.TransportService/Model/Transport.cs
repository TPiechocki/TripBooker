using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TripBooker.TransportService.Model;

internal class Transport
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int OptionId { get; set; }

    [ForeignKey("OptionId")]
    public TransportOption Option { get; set; } = null!;

    public DateOnly DepartureDate { get; set; }

    public int NumberOfSeats { get; set; }

    public bool IsReturn { get; set; }
}