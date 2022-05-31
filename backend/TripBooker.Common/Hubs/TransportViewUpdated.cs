using System;
using System.Collections.Generic;

namespace TripBooker.Common.Hubs;

public class TransportViewUpdated
{
    public TransportViewUpdated(IEnumerable<Guid> transportIds)
    {
        TransportIds = transportIds;
    }

    public IEnumerable<Guid> TransportIds { get; }
}