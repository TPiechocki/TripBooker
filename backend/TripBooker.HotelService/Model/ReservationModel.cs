using TripBooker.Common;
using TripBooker.Common.Hotel;

namespace TripBooker.HotelService.Model;

internal class ReservationModel : EventModel
{
    public IEnumerable<Guid> HotelDays { get; set; } = new List<Guid>();

    public int RoomsStudio { get; set; }

    public int RoomsSmall { get; set; }

    public int RoomsMedium { get; set; }

    public int RoomsLarge { get; set; }

    public int RoomsApartment { get; set; }

    public MealOption MealOption { get; set; }

    public double Price { get; set; }

    public ReservationStatus Status { get; set; }

    public string DestinationAirportCode { get; set; }
}