namespace TripBooker.Common.Statistics.Updates;

public class HotelCount
{
    public HotelCount(string destination, string hotelCode, int orderCount,
        int roomsStudio, int roomsSmall, int roomsMedium, int roomsLarge, int roomsApartment)
    {
        HotelCode = hotelCode;
        OrderCount = orderCount;
        RoomsStudio = roomsStudio;
        RoomsSmall = roomsSmall;
        RoomsMedium = roomsMedium;
        RoomsLarge = roomsLarge;
        RoomsApartment = roomsApartment;
        Destination = destination;
    }

    public string Destination { get; }

    public string HotelCode { get; }

    public int OrderCount { get; }

    public int RoomsStudio { get; }

    public int RoomsSmall { get; }

    public int RoomsMedium { get; }

    public int RoomsLarge { get; }

    public int RoomsApartment { get; }
}

public class GetHotelCount
{
    public string Destination { get; set; } = null!;

    public string HotelCode { get; set; } = null!;
}