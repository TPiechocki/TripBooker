using System.Collections.Generic;

namespace TripBooker.Common.TravelAgency.Contract.Query;

public class DestinationsQueryContract
{
    
}

public class DestinationsQueryResultContract
{
    public DestinationsQueryResultContract(IEnumerable<DestinationContract> destinations)
    {
        Destinations = destinations;
    }

    public IEnumerable<DestinationContract> Destinations { get; }
}

public class DestinationContract
{
    public DestinationContract(string airportCode, string name)
    {
        AirportCode = airportCode;
        Name = name;
    }

    public string AirportCode { get; }

    public string Name { get; }
}