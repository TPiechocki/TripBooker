namespace TripBooker.HotelService.Model;

internal static class HotelOccupationBuilder
{
    public static HotelOccupationModel buildNew(HotelOption hotelOption, DateTime day)
    {
        HotelOccupationModel model = new HotelOccupationModel
        {
            Date = day,
            HotelId = hotelOption.Id
        };

        foreach (RoomOption roomOption in hotelOption.Rooms)
        {
            switch(roomOption.RoomType)
            {
                case Common.Hotel.RoomType.Small:
                    model.RoomsSmall++;
                    break;
                case Common.Hotel.RoomType.Medium:
                    model.RoomsMedium++;
                    break;
                case Common.Hotel.RoomType.Large:
                    model.RoomsLarge++;
                    break;
                case Common.Hotel.RoomType.Apartment:
                    model.RoomsApartment++;
                    break;
                case Common.Hotel.RoomType.Studio:
                    model.RoomsStudio++;
                    break;
                default:
                    break;
            }
        }

        return model;
    }
}
