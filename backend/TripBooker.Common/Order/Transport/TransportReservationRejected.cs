using System;

namespace TripBooker.Common.Order.Transport;

public class TransportReservationRejected : ContractBase
{
    public TransportReservationRejected(
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