using System.Collections.Generic;

namespace TripBooker.Common.Statistics.Query;

public class GetHotelCountsQuery
{
    public string Destination { get; set; } = null!;
}

public class GetHotelCountsResponse
{
    public GetHotelCountsResponse(IReadOnlyDictionary<string, int> counts)
    {
        Counts = counts;
    }

    public IReadOnlyDictionary<string, int> Counts { get; }
}