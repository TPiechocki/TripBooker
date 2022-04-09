using System.Collections.Generic;

namespace TripBooker.Common.TravelAgency.Folder.Query;

public class DestinationsQueryContract
{
    
}

public class DestinationsQueryResultContract
{
    public DestinationsQueryResultContract(IEnumerable<string> destinations)
    {
        Destinations = destinations;
    }

    public IEnumerable<string> Destinations { get; set; }
}