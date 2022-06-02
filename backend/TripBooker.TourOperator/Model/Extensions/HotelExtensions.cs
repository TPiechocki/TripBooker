namespace TripBooker.TourOperator.Model.Extensions;

internal static class HotelExtensions
{
    public static HotelOccupationModel Reduce(this List<HotelOccupationModel> hotelDays) => ((IEnumerable<HotelOccupationModel>)hotelDays).Reduce();

    public static HotelOccupationModel Reduce(this IEnumerable<HotelOccupationModel> hotelDays)
    {
        var minvals = hotelDays.First();
        foreach (var hotelDay in hotelDays)
        {
            minvals.PriceModifier = Math.Min(minvals.PriceModifier, hotelDay.PriceModifier);
            minvals.RoomsStudio = Math.Min(minvals.RoomsStudio, hotelDay.RoomsStudio);
            minvals.RoomsSmall = Math.Min(minvals.RoomsSmall, hotelDay.RoomsSmall);
            minvals.RoomsMedium = Math.Min(minvals.RoomsMedium, hotelDay.RoomsMedium);
            minvals.RoomsLarge = Math.Min(minvals.RoomsLarge, hotelDay.RoomsLarge);
            minvals.RoomsApartment = Math.Min(minvals.RoomsApartment, hotelDay.RoomsApartment);
        }

        return minvals;
    }
}
