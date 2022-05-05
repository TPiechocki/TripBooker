using System;

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

    public Guid TransportId { get; set; }

    public Guid ReturnTransportId { get; set; }

    public Guid HotelId { get; set; }
    
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

    // RETURN TRANSPORT
    public double ReturnTransportPrice { get; set; }

    public Guid? ReturnTransportReservationId { get; set; }


    // HOTEL
    public Guid HotelReservationId { get; set; }

    public double HotelPrice { get; set; }

    #endregion

}