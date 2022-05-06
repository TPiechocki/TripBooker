using System;
using System.Collections.Generic;
using TripBooker.Common.Hotel.Contract;
using TripBooker.Common.TravelAgency.Model;

namespace TripBooker.Common.TravelAgency.Contract.Query;

public class TripOptionsQueryContract
{
    public string HotelCode { get; set; } = null!;

    public DateTime DepartureDate { get; set; }

    public int NumberOfDays { get; set; }
}

public class TripOptionsQueryResponse
{
    public IEnumerable<Guid> HotelDays { get; set; } = null!;

    public IEnumerable<TransportModel> TransportOptions { get; set; } = null!;

    public IEnumerable<TransportModel> ReturnTransportOptions { get; set; } = null!;

    public HotelOccupationViewContract HotelAvailability { get; set; } = null!;
}