namespace TripBooker.Common;

public enum PaymentStatus
{
    /// <summary>
    /// Fresh payment awaiting for action.
    /// </summary>
    New,

    /// <summary>
    /// Payment is accepted.
    /// </summary>
    Accepted,

    /// <summary>
    /// Payment is rejected.
    /// </summary>
    Rejected,

    /// <summary>
    /// Payment is in progress.
    /// </summary>
    InProgress,

    /// <summary>
    /// Payment was not done in time.
    /// </summary>
    Timeout,
}