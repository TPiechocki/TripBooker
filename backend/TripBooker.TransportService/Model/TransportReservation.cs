using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TripBooker.Common.Transport;

namespace TripBooker.TransportService.Model;

internal class TransportReservation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int TranportId { get; set; }

    [ForeignKey("TranportId")]
    public Transport Transport { get; set; } = null!;

    public int Places { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.New;
}