using System.Collections.Generic;

namespace TripBooker.Common.Statistics.Query;

public class GetDestinationCountsQuery
{
}

public class GetDestinationCountsResponse
{
    public GetDestinationCountsResponse(IReadOnlyDictionary<string, int> counts)
    {
        Counts = counts;
    }

    public IReadOnlyDictionary<string, int> Counts { get; }
}