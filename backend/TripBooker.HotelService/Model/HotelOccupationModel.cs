using TripBooker.Common;

namespace TripBooker.HotelService.Model;

internal class HotelOccupationModel : EventModel
{
    public int HotelId { get; set; }
    
    public DateTime Date { get; set; }

    public int RoomsStudio { get; set; }

    public int RoomsSmall { get; set; }

    public int RoomsMedium { get; set; }

    public int RoomsLarge { get; set; }

    public int RoomsApartment { get; set; }
}
