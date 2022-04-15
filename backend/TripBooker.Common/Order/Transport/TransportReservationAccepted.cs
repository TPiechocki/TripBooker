using System;

namespace TripBooker.Common.Order.Transport;

public class TransportReservationAccepted : ContractBase
{
    public TransportReservationAccepted(
        Guid correlationId,
        int price)
        : base(correlationId)
    {
        Price = price;
    }

    public int Price { get; }
}