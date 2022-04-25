using TripBooker.Common.Hotel.Contract;

namespace TripBooker.HotelService.Model.Mappings;

internal static class HotelOccupationViewContractMapper
{
    public static HotelOccupationViewContract MapFrom(HotelOccupationModel occupation, HotelOption hotel)
    {
        return new HotelOccupationViewContract
        {
            HotelId = occupation.HotelId,
            HotelCode = hotel.Code,
            HotelName = hotel.Name,
            Country = hotel.Country,
            AirportCode = hotel.AirportCode,
            Rating = hotel.Rating,
            AllInclusive = hotel.AllInclusive,
            Date = occupation.Date,
            RoomsStudio = occupation.RoomsStudio,
            RoomsSmall = occupation.RoomsSmall,
            RoomsMedium = occupation.RoomsMedium,
            RoomsLarge = occupation.RoomsLarge,
            RoomsApartment = occupation.RoomsApartment
        };
    }
}
