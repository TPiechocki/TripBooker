using System;
using System.ComponentModel.DataAnnotations;

namespace TripBooker.Common.Order;

public class OrderCommand
{
    public OrderData Order { get; set; } = null!;
}

public class OrderData
{
    /// <summary>
    /// Should be set before finalize to describe reason of the unsuccessful reservation.
    /// Null means successful action for the defined state of the saga.
    /// </summary>
    public string? FailureMessage { get; set; }

    public int Price => TransportPrice + ReturnTransportPrice;

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

    // TODO: analogical three fields for return transport
    public Guid TransportId { get; set; }

    public int TransportPrice { get; set; }

    public Guid? TransportReservationId { get; set; }

    public Guid ReturnTransportId { get; set; }

    public int ReturnTransportPrice { get; set; }

    public Guid? ReturnTransportReservationId { get; set; }
}