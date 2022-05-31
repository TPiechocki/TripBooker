using System;
using System.Collections.Generic;
using TripBooker.Common.Hotel;

namespace TripBooker.Common.Order;

public class OrderCommand
{
    public OrderData Order { get; set; } = null!;
}

public class OrderData
{
    #region InitialData

    public int NumberOfAdults { get; set; }

    public int NumberOfChildrenUpTo18 { get; set; }

    public int NumberOfChildrenUpTo10 { get; set; }

    public int NumberOfChildrenUpTo3 { get; set; }

    public string? DiscountCode { get; set; } = null!;

    /// <remarks>
    /// Null means no transport, and the clients would travel on their own.
    /// </remarks>
    public Guid? TransportId { get; set; }

    /// <remarks>
    /// Null means no transport, and the clients would travel on their own.
    /// </remarks>
    public Guid? ReturnTransportId { get; set; }

    public string HotelCode { get; set; } = null!;

    public IEnumerable<Guid> HotelDays { get; set; } = null!;

    public int RoomsStudio { get; set; }

    public int RoomsSmall { get; set; }

    public int RoomsMedium { get; set; }

    public int RoomsLarge { get; set; }

    public int RoomsApartment { get; set; }

    public MealOption MealOption { get; set; }

    public string? UserName { get; set; }

    #endregion


    #region SagaRuntimeData

    public Guid OrderId { get; set; }

    /// <summary>
    /// Should be set before finalize to describe reason of the unsuccessful reservation.
    /// Null means successful action for the defined state of the saga.
    /// </summary>
    public string? FailureMessage { get; set; }

    public double Price => TransportPrice + ReturnTransportPrice + HotelPrice;


    // TRANSPORT
    public double TransportPrice { get; set; }

    public Guid? TransportReservationId { get; set; }

    public string? DepartureAirportCode { get; set; }


    // RETURN TRANSPORT
    public double ReturnTransportPrice { get; set; }

    public Guid? ReturnTransportReservationId { get; set; }

    public string? ReturnAirportCode { get; set; }


    // HOTEL
    public Guid? HotelReservationId { get; set; }

    public double HotelPrice { get; set; }


    // STATISTICS
    public string? DestinationAirportCode { get; set; } = null!;

    #endregion
}