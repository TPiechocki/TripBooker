using TripBooker.Common.Hotel;

namespace TripBooker.HotelService.Contract;

internal class NewReservationContract
{
    public NewReservationContract(IEnumerable<Guid> hotelDays, int studio, int small, int medium, int large, int apartment)
    {
        HotelDays = hotelDays;
        RoomsStudio = studio;
        RoomsSmall = small;
        RoomsMedium = medium;
        RoomsLarge = large;
        RoomsApartment = apartment;
    }

    public IEnumerable<Guid> HotelDays { get; set; }

    public int RoomsStudio { get; set; }

    public int RoomsSmall { get; set; }

    public int RoomsMedium { get; set; }

    public int RoomsLarge { get; set; }

    public int RoomsApartment { get; set; }
}
