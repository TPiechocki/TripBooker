using System;

namespace TripBooker.Common.Hotel.Contract.Command;

public class CancelReservationContract : ContractBase
{
    public CancelReservationContract(Guid correlationId, Guid reservationId)
        : base(correlationId)
    {
        ReservationId = reservationId;
    }

    public Guid ReservationId { get; }
}
