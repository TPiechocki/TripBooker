﻿using System;

namespace TripBooker.Common.Order.Transport;

public class TransportReservationAccepted : ContractBase
{
    public TransportReservationAccepted(
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