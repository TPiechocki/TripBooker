using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TripBooker.Common;

public class EventTimestamp
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int BsonId { get; set; }

    public string Id { get; set; } = null!;

    public DateTime Timestamp { get; set; }
}
