using System;

namespace TripBooker.Common;

public abstract class EventModel
{
    public Guid Id { get; set; }

    public int Version { get; set; }
}