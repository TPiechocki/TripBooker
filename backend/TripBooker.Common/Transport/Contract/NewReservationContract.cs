using System;

namespace TripBooker.Common.Transport.Contract;

public class NewReservationContract : ContractBase
{
    public NewReservationContract(Guid correlationId, Guid transportId, int places) 
        : base(correlationId)
    {
        TransportId = transportId;
        Places = places;
    }

    public Guid TransportId { get; }

    public int Places { get; }
}