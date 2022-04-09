using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TripBooker.Common.Transport;

namespace TripBooker.TransportService.Model;

internal class TransportOption
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string DepartureAirportCode { get; set; } = null!;

    public string DepartureAirportName { get; set; } = null!;

    public string DepartureAirportCountry { get; set; } = null!;

    [Required]
    public string DestinationAirportCode { get; set; } = null!;

    public string DestinationAirportName { get; set; } = null!;
    
    public string DestinationAirportCountry { get; set; } = null!;

    [Required]
    public TransportType Type { get; set; }

    /// <summary>
    /// Transport duration in minutes
    /// </summary>
    public int Duration { get; set; }
}