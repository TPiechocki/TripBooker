using System;
using System.Collections.Generic;

namespace TripBooker.Common.Hubs;

public class HotelViewUpdated
{
    public HotelViewUpdated(IEnumerable<Guid> hotelDayIds
    )
    {
        HotelDayIds = hotelDayIds;
    }

    public IEnumerable<Guid> HotelDayIds { get; }
}