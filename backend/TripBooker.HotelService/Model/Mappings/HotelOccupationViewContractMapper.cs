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
            BreakfestPrice = hotel.PriceModifier * HotelConstants.BaseBreakfeastPrice,
            AllInclusive = hotel.AllInclusive,
            AllInclusivePrice = hotel.PriceModifier * HotelConstants.BaseAllInclusivePrice,
            Date = occupation.Date,
            RoomsStudio = occupation.RoomsStudio,
            StudioPrice = hotel.PriceModifier 
                          * hotel.Rooms.Where(r => r.RoomType == RoomType.Studio)
                                       .FirstOrDefault(new RoomOption { PriceModifier = 1.0}).PriceModifier 
                          * HotelConstants.BaseRoomPrice,
            RoomsSmall = occupation.RoomsSmall,
            SmallPrice = hotel.PriceModifier
                         * hotel.Rooms.Where(r => r.RoomType == RoomType.Small)
                                      .FirstOrDefault(new RoomOption { PriceModifier = 1.0 }).PriceModifier
                         * HotelConstants.BaseRoomPrice,
            RoomsMedium = occupation.RoomsMedium,
            MediumPrice = hotel.PriceModifier
                          * hotel.Rooms.Where(r => r.RoomType == RoomType.Medium)
                                       .FirstOrDefault(new RoomOption { PriceModifier = 1.0 }).PriceModifier
                          * HotelConstants.BaseRoomPrice,
            RoomsLarge = occupation.RoomsLarge,
            LargePrice = hotel.PriceModifier
                         * hotel.Rooms.Where(r => r.RoomType == RoomType.Large)
                                      .FirstOrDefault(new RoomOption { PriceModifier = 1.0 }).PriceModifier
                         * HotelConstants.BaseRoomPrice,
            RoomsApartment = occupation.RoomsApartment,
            ApartmentPrice = hotel.PriceModifier
                             * hotel.Rooms.Where(r => r.RoomType == RoomType.Apartment)
                                          .FirstOrDefault(new RoomOption { PriceModifier = 1.0 }).PriceModifier
                             * HotelConstants.BaseRoomPrice
        };
    }
}
