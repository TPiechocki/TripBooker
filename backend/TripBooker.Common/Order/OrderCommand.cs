using System;
using System.ComponentModel.DataAnnotations;

namespace TripBooker.Common.Order;

public class OrderCommand
{
    public OrderData Order { get; set; } = null!;
}

public class OrderData
{
    public int NumberOfAdults { get; set; }

    public int NumberOfChildrenUpTo18 { get; set; }

    public int NumberOfChildrenUpTo10 { get; set; }

    public int NumberOfChildrenUpTo3 { get; set; }

    /// <summary>
    /// Number of places to occupy in transport.
    /// Children up to 3 years don't take place and travel with parent on one seat.
    /// </summary>
    public int NumberOfOccupiedSeats => NumberOfAdults + NumberOfChildrenUpTo18 + NumberOfChildrenUpTo10;

    public Guid OrderId { get; set; }

    public Guid TransportId { get; set; }

    public int TransportPrice { get; set; }
}