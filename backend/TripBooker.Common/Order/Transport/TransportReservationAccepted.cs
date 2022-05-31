using System;

namespace TripBooker.Common.Order.Transport;

public class TransportReservationAccepted : ContractBase
{
    public TransportReservationAccepted(
        Guid correlationId,
        double price,
        Guid reservationId,
        string? localAirportCode)
        : base(correlationId)
    {
        Price = price;
        ReservationId = reservationId;
        LocalAirportCode = localAirportCode;
    }

    public double Price { get; }

    public Guid ReservationId { get; }

    public string? LocalAirportCode { get; }
}