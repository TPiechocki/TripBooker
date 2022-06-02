using System.ComponentModel.DataAnnotations;

namespace TripBooker.StatisticsService.Model;

public class ReservationModel
{
    [Key]
    public Guid OrderId { get; set; }

    public DateTime TimeStamp { get; set; }

    public string DestinationAirportCode { get; set; } = null!;

    public string HotelCode { get; set; } = null!;

    public string? DepartureAirportCode { get; set; }

    public string? ReturnAirportCode { get; set; }

    public int RoomsStudio { get; set; }

    public int RoomsSmall { get; set; }

    public int RoomsMedium { get; set; }

    public int RoomsLarge { get; set; }

    public int RoomsApartment { get; set; }
}