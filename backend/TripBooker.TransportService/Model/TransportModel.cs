using TripBooker.Common;

namespace TripBooker.TransportService.Model;

internal class TransportModel : EventModel
{
    public DateTime DepartureDate { get; set; }

    public int TransportOptionId { get; set; }
    
    public int AvailablePlaces { get; set; }

    public int TicketPrice { get; set; }
}