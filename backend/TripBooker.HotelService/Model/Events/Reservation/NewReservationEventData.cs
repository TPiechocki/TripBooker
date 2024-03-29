﻿using TripBooker.Common.Hotel;

namespace TripBooker.HotelService.Model.Events.Reservation;

public class NewReservationEventData
{
    public NewReservationEventData(
        IEnumerable<Guid> hotelDays,
        int studio,
        int small,
        int medium,
        int large,
        int apartment,
        MealOption mealOption)
    {
        HotelDays = hotelDays;
        RoomsStudio = studio;
        RoomsSmall = small;
        RoomsMedium = medium;
        RoomsLarge = large;
        RoomsApartment = apartment;
        MealOption = mealOption;
    }

    public IEnumerable<Guid> HotelDays { get; set; }

    public int RoomsStudio { get; set; }

    public int RoomsSmall { get; set; }

    public int RoomsMedium { get; set; }

    public int RoomsLarge { get; set; }

    public int RoomsApartment { get; set; }

    public MealOption MealOption { get; set; }
}
