using System;
using System.Collections.Generic;

namespace TripBooker.Common.Hubs;

public class PurchasedOfferNotification
{
    public Guid OrderId { get; set; }

    public IEnumerable<Guid> PurchasedHotelDays { get; set; } = null!;
}