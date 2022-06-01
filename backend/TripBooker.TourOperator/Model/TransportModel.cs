using System.ComponentModel.DataAnnotations;

namespace TripBooker.TourOperator.Model;

public class TransportModel
{
    [Key]
    public Guid Id { get; set; }

    public int TicketPrice { get; set; }

    public int AvailablePlaces { get; set; }
}
