using System;

namespace TripBooker.Common.Transport.Contract.Response;

public class ReservationRejectedContract : ContractBase
{
    public ReservationRejectedContract(Guid correlationId)
        : base(correlationId)
    {
    }
}