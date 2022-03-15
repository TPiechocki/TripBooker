namespace TripBooker.Common.Transport;

public enum ReservationStatus
{
    /// <summary>
    /// Fresh reservation which was not yet checked with state of available places.
    /// </summary>
    New,

    /// <summary>
    /// Reservation has reserved places and is waiting for the payment.
    /// </summary>
    AwaitingPayment,
    
    /// <summary>
    /// Reservation is paid and confirmed.
    /// </summary>
    Confirmed,

    /// <summary>
    /// Reservation could not be fulfilled with current state of available places.
    /// </summary>
    Rejected,

    /// <summary>
    /// Payment was rejected or payment timeout was hit, so the reserved places were set free.
    /// </summary>
    PaymentRejected
}