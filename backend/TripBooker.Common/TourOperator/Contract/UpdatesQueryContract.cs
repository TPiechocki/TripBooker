using System;
using System.Collections.Generic;

namespace TripBooker.Common.TourOperator.Contract.Query;

public class UpdatesQueryContract
{

}

public class UpdatesQueryResultContract
{
    public UpdatesQueryResultContract(IEnumerable<UpdateContract> updates)
    {
        Updates = updates;
    }

    public IEnumerable<UpdateContract> Updates { get; }
}

public class UpdateContract
{
    public UpdateContract(DateTime timestamp, string description)
    {
        Timestamp = timestamp;
        Description = description;
    }

    DateTime Timestamp { get; }

    string Description { get; }
}
