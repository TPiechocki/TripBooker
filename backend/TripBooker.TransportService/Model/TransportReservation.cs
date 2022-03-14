using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TripBooker.Common.Transport;

namespace TripBooker.TransportService.Model;

internal class TransportReservations
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int TransportId { get; set; }

    public ICollection<TransportReservation> Reservations { get; set; } = null!;
}

internal class TransportReservation
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    public ReservationStatus Status { get; set; }

    public int Places { get; set; }

    public ICollection<ReservationStatusChange> StatusHistory { get; set; } = null!;
}

internal class ReservationStatusChange
{
    public ReservationStatus NewStatus { get; }

    public DateTime TimeStamp { get; set; }

    public ReservationStatusChange(ReservationStatus newStatus)
    {
        NewStatus = newStatus;
        TimeStamp = DateTime.UtcNow;
    }
}