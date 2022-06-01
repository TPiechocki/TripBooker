using System;

namespace TripBooker.Common.TourOperator.Contract;

public class TransportUpdateContract
{
    public Guid Id { get; set; }

    public bool PriceChangedFlag { get; set; } = false;

    public int NewTicketPrice { get; set; }

    public int AvailablePlacesChange { get; set; } = 0;

}
