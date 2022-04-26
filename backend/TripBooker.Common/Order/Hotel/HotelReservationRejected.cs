using System;

namespace TripBooker.Common.Order.Hotel;

public class HotelReservationRejected : ContractBase
{
    public HotelReservationRejected(
        Guid correlationId,
        Guid? reservationId)
        : base(correlationId)
    {
        ReservationId = reservationId;
    }

    /// <summary>
    /// Id of the rejected reservation.
    /// Can be null for unknown and unhandled errors when only correlation id is known.
    /// </summary>
    public Guid? ReservationId { get; }
}
