using TripBooker.Common.Hotel.Contract;

namespace TripBooker.TourOperator.Model.Mappings;

internal static class HotelOccupationModelMapper
{
    public static HotelOccupationModel MapFrom(HotelOccupationViewContract contract)
    {
        var model = new HotelOccupationModel()
        {
            HotelId = contract.HotelId,
            Date = contract.Date,
            RoomsStudio = contract.RoomsStudio,
            RoomsSmall = contract.RoomsSmall,
            RoomsMedium = contract.RoomsMedium,
            RoomsLarge = contract.RoomsLarge,
            RoomsApartment = contract.RoomsApartment,
        };
        return model;
    }
}
