using System;

namespace TripBooker.Common.Order.Hotel;

public class CancelHotelReservation : ContractBase
{
    public CancelHotelReservation(Guid correlationId, Guid reservationId)
        : base(correlationId)
    {
        ReservationId = reservationId;
    }

    public Guid ReservationId { get; }
}
