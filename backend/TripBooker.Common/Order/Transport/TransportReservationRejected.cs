using System;

namespace TripBooker.Common.Order.Transport;

public class TransportReservationRejected : ContractBase
{
    public TransportReservationRejected(Guid correlationId)
        : base(correlationId)
    {
    }
}