using System;

namespace TripBooker.Common;

public abstract class ContractBase
{
    protected ContractBase(Guid correlationId)
    {
        CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }
}