using System;

namespace TripBooker.Common.Transport.Contract;

public class ReservationRejectedContract : ContractBase
{
    public ReservationRejectedContract(Guid correlationId)
        : base(correlationId)
    {
    }
}