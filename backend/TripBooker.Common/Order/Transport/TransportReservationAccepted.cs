using System;

namespace TripBooker.Common.Order.Transport;

public class TransportReservationAccepted : ContractBase
{
    public TransportReservationAccepted(Guid correlationId)
        : base(correlationId)
    {
    }
}