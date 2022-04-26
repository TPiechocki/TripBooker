using System;
using System.Collections.Generic;
using TripBooker.Common.Hotel;

namespace TripBooker.Common.Order.Hotel;

public class NewHotelReservation : OrderCommand
{
    public NewHotelReservation(
        IEnumerable<Guid> hotelDays,
        int roomsStudio,
        int roomsSmall,
        int roomsMedium,
        int roomsLarge,
        int roomsApartment)
    {
        HotelDays = hotelDays;
        RoomsStudio = roomsStudio;
        RoomsSmall = roomsSmall;
        RoomsMedium = roomsMedium;
        RoomsLarge = roomsLarge;
        RoomsApartment = roomsApartment;
    }

    public IEnumerable<Guid> HotelDays { get; set; }

    public int RoomsStudio { get; set; }

    public int RoomsSmall { get; set; }

    public int RoomsMedium { get; set; }

    public int RoomsLarge { get; set; }

    public int RoomsApartment { get; set; }

    public MealOption MealOption { get; set; }
}
