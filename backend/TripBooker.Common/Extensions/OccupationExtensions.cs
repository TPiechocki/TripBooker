using TripBooker.Common.Order;
using TripBooker.Common.TravelAgency.Contract.Query;

namespace TripBooker.Common.Extensions;

public static class OccupationExtensions
{
    public static int NumberOfHotelPlaces(this TripsQueryContract data)
    {
        return NumberOfHotelPlaces(data.NumberOfAdults, data.NumberOfChildrenUpTo18, data.NumberOfChildrenUpTo10,
            data.NumberOfChildrenUpTo3);
    }

    public static int NumberOfOccupiedSeats(this TripsQueryContract data)
    {
        return NumberOfOccupiedSeats(data.NumberOfAdults, data.NumberOfChildrenUpTo18, data.NumberOfChildrenUpTo10);
    }


    public static int NumberOfHotelPlaces(this OrderData data)
    {
        return NumberOfHotelPlaces(data.NumberOfAdults, data.NumberOfChildrenUpTo18, data.NumberOfChildrenUpTo10,
            data.NumberOfChildrenUpTo3);
    }

    public static int NumberOfOccupiedSeats(this OrderData data)
    {
        return NumberOfOccupiedSeats(data.NumberOfAdults, data.NumberOfChildrenUpTo18, data.NumberOfChildrenUpTo10);
    }


    public static int NumberOfOccupiedSeats(this TripQueryContract data)
    {
        return NumberOfOccupiedSeats(data.NumberOfAdults, data.NumberOfChildrenUpTo18, data.NumberOfChildrenUpTo10);
    }

    public static int NumberOfHotelPlaces(this TripQueryContract data)
    {
        return NumberOfHotelPlaces(data.NumberOfAdults, data.NumberOfChildrenUpTo18, data.NumberOfChildrenUpTo10,
            data.NumberOfChildrenUpTo3);
    }


    private static int NumberOfHotelPlaces(int numberOfAdults, int numberOfChildrenUpTo18, int numberOfChildrenUpTo10,
        int numberOfChildrenUpTo3)
    {
        return numberOfAdults + numberOfChildrenUpTo18 + numberOfChildrenUpTo10 + numberOfChildrenUpTo3;
    }

    /// <summary>
    /// Number of places to occupy in transport.
    /// Children up to 3 years don't take place and travel with parent on one seat.
    /// </summary>
    private static int NumberOfOccupiedSeats(int numberOfAdults, int numberOfChildrenUpTo18, int numberOfChildrenUpTo10)
    {
        return numberOfAdults + numberOfChildrenUpTo18 + numberOfChildrenUpTo10;
    }
}