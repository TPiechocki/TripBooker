using TripBooker.Common.Hotel;
using TripBooker.Common.Order;
using TripBooker.HotelService.Model.Events.Hotel;

namespace TripBooker.HotelService.Model.Extensions;

internal static class HotelExtensions
{
    public static NewHotelDayEventData MapToNewHotelDayEventData(DateTime date, HotelOption hotel)
    {
        NewHotelDayEventData eventData = new NewHotelDayEventData
        {
            Date = date,
            HotelId = hotel.Id
        };

        foreach (RoomOption roomOption in hotel.Rooms)
        {
            switch (roomOption.RoomType)
            {
                case RoomType.Small:
                    eventData.RoomsSmall++;
                    break;
                case RoomType.Medium:
                    eventData.RoomsMedium++;
                    break;
                case RoomType.Large:
                    eventData.RoomsLarge++;
                    break;
                case RoomType.Apartment:
                    eventData.RoomsApartment++;
                    break;
                case RoomType.Studio:
                    eventData.RoomsStudio++;
                    break;
                default:
                    break;
            }
        }

        return eventData;
    }

    public static double CalculatePrice(OrderData order, IEnumerable<HotelOccupationModel> occupationModels, HotelOption hotel)
    {
        // Rooms
        var price = order.RoomsStudio * hotel.GetPriceFor(RoomType.Studio)
                    + order.RoomsSmall * hotel.GetPriceFor(RoomType.Small)
                    + order.RoomsMedium * hotel.GetPriceFor(RoomType.Medium)
                    + order.RoomsLarge * hotel.GetPriceFor(RoomType.Large)
                    + order.RoomsApartment * hotel.GetPriceFor(RoomType.Apartment);

        // Meals
        var mealPrice = hotel.GetPriceFor(order.MealOption);
        price += order.NumberOfAdults * mealPrice;
        price += order.NumberOfChildrenUpTo18 * HotelConstants.MealChildren10To18PriceFactor * mealPrice;
        price += (order.NumberOfChildrenUpTo3
                  + order.NumberOfChildrenUpTo10) * HotelConstants.MealChildrenUnder10PriceFactor * mealPrice;

        // Multiply by number of days
        price *= occupationModels.Sum(x => x.PriceModifier);

        return price;
    }
}
