using System;

namespace TripBooker.Common.Order.Hotel;

public class HotelReservationAccepted : ContractBase
{
    public HotelReservationAccepted(
        Guid correlationId,
        double price,
        Guid reservationId,
        string destinationAirportCode)
        : base(correlationId)
    {
        Price = price;
        ReservationId = reservationId;
        DestinationAirportCode = destinationAirportCode;
    }

    public double Price { get; }

    public Guid ReservationId { get; }

    public string DestinationAirportCode { get; }
}