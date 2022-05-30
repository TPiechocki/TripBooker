using TripBooker.Common.Hotel;
using TripBooker.Common.Hotel.Contract;

namespace TripBooker.HotelService.Model.Mappings;

internal static class HotelOccupationViewContractMapper
{
    public static HotelOccupationViewContract MapFrom(HotelOccupationModel occupation, HotelOption hotel)
    {
        return new HotelOccupationViewContract
        {
            Id = occupation.Id,
            HotelId = occupation.HotelId,
            HotelCode = hotel.Code,
            HotelName = hotel.Name,
            Country = hotel.Country,
            AirportCode = hotel.AirportCode,
            Rating = hotel.Rating,
            BreakfastPrice = hotel.GetPriceFor(MealOption.ContinentalBreakfeast) * occupation.PriceModifier,
            AllInclusive = hotel.AllInclusive,
            AllInclusivePrice = hotel.GetPriceFor(MealOption.AllInclusive) * occupation.PriceModifier,
            Date = occupation.Date,
            RoomsStudio = occupation.RoomsStudio,
            StudioPrice = hotel.GetPriceFor(RoomType.Studio) * occupation.PriceModifier,
            RoomsSmall = occupation.RoomsSmall,
            SmallPrice = hotel.GetPriceFor(RoomType.Small) * occupation.PriceModifier,
            RoomsMedium = occupation.RoomsMedium,
            MediumPrice = hotel.GetPriceFor(RoomType.Medium) * occupation.PriceModifier,
            RoomsLarge = occupation.RoomsLarge,
            LargePrice = hotel.GetPriceFor(RoomType.Large) * occupation.PriceModifier,
            RoomsApartment = occupation.RoomsApartment,
            ApartmentPrice = hotel.GetPriceFor(RoomType.Apartment) * occupation.PriceModifier,
        };
    }
}
