using System.Collections.Generic;

namespace TripBooker.Common.Statistics;

public class GetDestinationCountsQuery
{
}

public class GetDestinationCountsResponse
{
    public GetDestinationCountsResponse(IReadOnlyDictionary<string, int> counters)
    {
        Counters = counters;
    }

    public IReadOnlyDictionary<string, int> Counters { get; }
}