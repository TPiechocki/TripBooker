using System;
using System.Collections.Generic;

namespace TripBooker.Common.Hubs;

public class PurchasedOfferNotification
{
    public IEnumerable<Guid> PurchasedHotelDays { get; set; } = null!;
}