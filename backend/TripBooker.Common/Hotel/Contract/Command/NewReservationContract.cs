using System;
using System.Collections.Generic;

namespace TripBooker.Common.Hotel.Contract.Command;

public class NewReservationContract : ContractBase
{
    public NewReservationContract(Guid correlationId,
                                  IEnumerable<Guid> hotelDays,
                                  int roomsStudio,
                                  int roomsSmall,
                                  int roomsMedium,
                                  int roomsLarge,
                                  int roomsApartment) : base(correlationId)
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
}
