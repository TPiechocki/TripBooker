using System;

namespace TripBooker.Common.Order.Transport;

public class ConfirmTransportReservation : ContractBase
{
    public ConfirmTransportReservation(Guid correlationId, Guid reservationId) 
        : base(correlationId)
    {
        ReservationId = reservationId;
    }

    public Guid ReservationId { get; }
}