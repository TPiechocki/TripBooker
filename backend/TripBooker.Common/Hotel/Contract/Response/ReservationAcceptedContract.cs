using System;

namespace TripBooker.Common.Hotel.Contract.Response;

public class ReservationAcceptedContract : ContractBase
{
    public ReservationAcceptedContract(Guid correlationId)
        : base(correlationId)
    {
    }
}
