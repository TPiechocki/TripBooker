using System;

namespace TripBooker.Common.Order.Transport;

public class TransportReservationAccepted : ContractBase
{
    public TransportReservationAccepted(
        Guid correlationId,
        int price, 
        Guid reservationId)
        : base(correlationId)
    {
        Price = price;
        ReservationId = reservationId;
    }

    public int Price { get; }

    public Guid ReservationId { get; }
}