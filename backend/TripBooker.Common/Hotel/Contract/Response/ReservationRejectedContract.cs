using System;

namespace TripBooker.Common.Hotel.Contract.Response;

public class ReservationRejectedContract : ContractBase
{
    public ReservationRejectedContract(Guid correlationId)
        : base(correlationId)
    {
    }
}
