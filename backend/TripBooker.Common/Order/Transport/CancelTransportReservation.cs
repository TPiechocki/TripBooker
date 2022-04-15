using System;

namespace TripBooker.Common.Order.Transport;

public class CancelTransportReservation : ContractBase
{
    public CancelTransportReservation(Guid correlationId, Guid reservationId) 
        : base(correlationId)
    {
        ReservationId = reservationId;
    }

    public Guid ReservationId { get; }
}