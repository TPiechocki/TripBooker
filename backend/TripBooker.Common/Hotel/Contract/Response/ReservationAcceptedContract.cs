using System;

namespace TripBooker.Common.Hotel.Contract.Response;

public class ReservationAcceptedContract : ContractBase
{
    public ReservationAcceptedContract(Guid correlationId, Guid reservationId) 
        : base(correlationId) => ReservationId = reservationId;

    // TODO: Price

    public Guid ReservationId { get; }
}
