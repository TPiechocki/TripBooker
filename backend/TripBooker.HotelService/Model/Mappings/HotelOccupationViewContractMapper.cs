using TripBooker.Common.Hotel;
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
            BreakfestPrice = hotel.GetPriceFor(MealOption.ContinentalBreakfeast),
            AllInclusive = hotel.AllInclusive,
            AllInclusivePrice = hotel.GetPriceFor(MealOption.AllInclusive),
            Date = occupation.Date,
            RoomsStudio = occupation.RoomsStudio,
            StudioPrice = hotel.GetPriceFor(RoomType.Studio),
            RoomsSmall = occupation.RoomsSmall,
            SmallPrice = hotel.GetPriceFor(RoomType.Small),
            RoomsMedium = occupation.RoomsMedium,
            MediumPrice = hotel.GetPriceFor(RoomType.Medium),
            RoomsLarge = occupation.RoomsLarge,
            LargePrice = hotel.GetPriceFor(RoomType.Large),
            RoomsApartment = occupation.RoomsApartment,
            ApartmentPrice = hotel.GetPriceFor(RoomType.Apartment),
        };
    }
}
