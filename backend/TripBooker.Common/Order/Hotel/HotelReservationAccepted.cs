using System;

namespace TripBooker.Common.Order.Hotel;

public class HotelReservationAccepted : ContractBase
{
    public HotelReservationAccepted(
        Guid correlationId,
        double price,
        Guid reservationId)
        : base(correlationId)
    {
        Price = price;
        ReservationId = reservationId;
    }

    public double Price { get; }

    public Guid ReservationId { get; }
}
