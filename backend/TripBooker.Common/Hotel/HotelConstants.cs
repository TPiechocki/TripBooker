namespace TripBooker.Common.Hotel;

public static class HotelConstants
{
    public const double BaseRoomPrice = 50;

    public const double BaseBreakfeastPrice = 20;

    public const double BaseAllInclusivePrice = 100;

    public static int GetMaxPeople(this RoomType roomType) => roomType switch
    {
        RoomType.Studio => 1,
        RoomType.Small => 2,
        RoomType.Medium => 4,
        RoomType.Large => 8,
        RoomType.Apartment => 4,
        _ => 1
    };
}
