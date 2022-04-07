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
                case Common.Hotel.RoomType.Small:
                    eventData.RoomsSmall++;
                    break;
                case Common.Hotel.RoomType.Medium:
                    eventData.RoomsMedium++;
                    break;
                case Common.Hotel.RoomType.Large:
                    eventData.RoomsLarge++;
                    break;
                case Common.Hotel.RoomType.Apartment:
                    eventData.RoomsApartment++;
                    break;
                case Common.Hotel.RoomType.Studio:
                    eventData.RoomsStudio++;
                    break;
                default:
                    break;
            }
        }

        return eventData;
    }
}
