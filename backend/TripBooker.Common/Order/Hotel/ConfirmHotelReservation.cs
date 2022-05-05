using System;

namespace TripBooker.Common.Order.Hotel;

public class ConfirmHotelReservation : ContractBase
{
    public ConfirmHotelReservation(Guid correlationId, Guid reservationId)
        : base(correlationId)
    {
        ReservationId = reservationId;
    }

    public Guid ReservationId { get; }
}
