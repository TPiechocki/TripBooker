using TripBooker.Common.Extensions;
using TripBooker.Common.Hotel;
using TripBooker.Common.TravelAgency.Contract.Query;

namespace TripBooker.TravelAgencyService.Model;

internal static class ModelExtensions
{
    public static HotelOccupationModel ReduceHotelOccupations(this IReadOnlyCollection<HotelOccupationModel> hotelDays)
    {
        var reducedModel = hotelDays.First();

        reducedModel.RoomsSmall = hotelDays.Min(x => x.RoomsSmall);
        reducedModel.SmallPrice = hotelDays.Sum(x => x.SmallPrice);
        reducedModel.RoomsMedium = hotelDays.Min(x => x.RoomsMedium);
        reducedModel.MediumPrice = hotelDays.Sum(x => x.MediumPrice);
        reducedModel.RoomsLarge = hotelDays.Min(x => x.RoomsLarge);
        reducedModel.LargePrice = hotelDays.Sum(x => x.LargePrice);
        reducedModel.RoomsApartment = hotelDays.Min(x => x.RoomsApartment);
        reducedModel.ApartmentPrice = hotelDays.Sum(x => x.ApartmentPrice);
        reducedModel.RoomsStudio = hotelDays.Min(x => x.RoomsStudio);
        reducedModel.StudioPrice = hotelDays.Sum(x => x.StudioPrice);

        return reducedModel;
    }

    public static bool EnoughPlacesInConfiguration(this TripQueryContract data)
    {
        var maxSpotsInRooms = RoomType.Apartment.GetMaxPeople() * data.NumberOfApartments
                              + RoomType.Large.GetMaxPeople() * data.NumberOfLargeRooms
                              + RoomType.Medium.GetMaxPeople() * data.NumberOfMediumRooms
                              + RoomType.Small.GetMaxPeople() * data.NumberOfSmallRooms
                              + RoomType.Studio.GetMaxPeople() * data.NumberOfStudios;

        return maxSpotsInRooms >= data.NumberOfHotelPlaces();
    }
}