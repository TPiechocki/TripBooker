using System;

namespace TripBooker.Common.Transport.Contract.Response;

public class ReservationAcceptedContract : ContractBase
{
    public ReservationAcceptedContract(Guid correlationId)
        : base(correlationId)
    {
    }
}