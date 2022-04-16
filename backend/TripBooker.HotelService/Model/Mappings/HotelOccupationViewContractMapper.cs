using TripBooker.Common.Hotel.Contract;

namespace TripBooker.HotelService.Model.Mappings;

internal static class HotelOccupationViewContractMapper
{
    public static HotelOccupationViewContract MapFrom(HotelOccupationModel occupation, HotelOption hotel)
    {
        return new HotelOccupationViewContract
        {
            HotelId = occupation.HotelId,
            HotelName = hotel.Name,
            City = hotel.City,
            Address = hotel.Address,
            Rating = hotel.Rating,
            Date = occupation.Date,
            RoomsStudio = occupation.RoomsStudio,
            RoomsSmall = occupation.RoomsSmall,
            RoomsMedium = occupation.RoomsMedium,
            RoomsLarge = occupation.RoomsLarge,
            RoomsApartment = occupation.RoomsApartment
        };
    }
}
