using System;

namespace TripBooker.Common.TourOperator.Contract.Query;

public class TransportUpdateQuery
{
    public Guid Id { get; set; }

    public int NewTicketPrice { get; set; } = 0;

    public int AvailablePlacesChange { get; set; } = 0;
}
