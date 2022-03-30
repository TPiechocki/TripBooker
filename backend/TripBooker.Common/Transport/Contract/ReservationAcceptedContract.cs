using System;

namespace TripBooker.Common.Transport.Contract;

public class ReservationAcceptedContract : ContractBase
{
    public ReservationAcceptedContract(Guid correlationId)
        : base(correlationId)
    {
    }
}