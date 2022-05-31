using System.ComponentModel.DataAnnotations;

namespace TripBooker.StatisticsService.Model;

public class ReservationModel
{
    [Key]
    public Guid OrderId { get; set; }
    
    public DateTime TimeStamp { get; set; }

    public string DestinationAirportCode { get; set; } = null!;
}