using TripBooker.Common.Hotel;

namespace TripBooker.TravelAgencyService.Model;

internal class HotelOccupationModel
{
    public Guid Id { get; set; }

    public string AirportCode { get; set; } = null!;
    public bool AllInclusive { get; set; }
    public double AllInclusivePrice { get; set; }
    public double ApartmentPrice { get; set; }
    public double BreakfastPrice { get; set; }
    public string Country { get; set; } = null!;
    public DateTime Date { get; set; }
    public string HotelCode { get; set; } = null!;
    public Guid HotelId { get; set; }
    public string HotelName { get; set; } = null!;
    public double LargePrice { get; set; }
    public double MediumPrice { get; set; }
    public float Rating { get; set; }
    public int RoomsApartment { get; set; }
    public int RoomsLarge { get; set; }
    public int RoomsMedium { get; set; }
    public int RoomsSmall { get; set; }
    public int RoomsStudio { get; set; }
    public double SmallPrice { get; set; }
    public double StudioPrice { get; set; }

    public int MaxNumberOfPeople =>
        RoomType.Apartment.GetMaxPeople() * RoomsApartment 
        + RoomType.Large.GetMaxPeople() * RoomsLarge 
        + RoomType.Medium.GetMaxPeople() * RoomsMedium 
        + RoomType.Small.GetMaxPeople() * RoomsSmall 
        + RoomType.Studio.GetMaxPeople() * RoomsStudio;

    public double GetMinPrice(int numberOfPeople, int numberOfDays = 1)
    {
        var np = numberOfPeople;
        var minPrice = 0.0;

        var nLarge = Math.Min(np / RoomType.Large.GetMaxPeople(), RoomsLarge);
        np -= nLarge * RoomType.Large.GetMaxPeople();
        minPrice += nLarge * LargePrice;
        // Check if can fit remaining people in one more Large room
        if (nLarge < RoomsLarge && np > RoomType.Medium.GetMaxPeople())
            return (minPrice + LargePrice) * numberOfDays;

        var nMedium = Math.Min(np / RoomType.Medium.GetMaxPeople(), RoomsMedium);
        np -= nMedium * RoomType.Medium.GetMaxPeople();
        minPrice += nMedium * MediumPrice;
        // Check if can fit remaining people in one more Medium room
        if (nMedium < RoomsMedium && np > RoomType.Small.GetMaxPeople())
            return (minPrice + MediumPrice) * numberOfDays;

        var nSmall = Math.Min(np / RoomType.Small.GetMaxPeople(), RoomsSmall);
        np -= nSmall * RoomType.Small.GetMaxPeople();
        minPrice += nSmall * SmallPrice;
        // Check if can fit remaining people in one more Small room
        if (nSmall < RoomsSmall && np > RoomType.Studio.GetMaxPeople())
            return (minPrice + SmallPrice) * numberOfDays;

        var nStudio = Math.Min(np / RoomType.Studio.GetMaxPeople(), RoomsStudio);
        np -= nStudio * RoomType.Studio.GetMaxPeople();
        minPrice += nStudio * StudioPrice;

        var nApartment = 0;
        // If there is no other option put people with filling rooms partially or Apartments
        while (np > 0)
        {
            if (RoomsSmall - nSmall > 0)
            {
                nSmall++;
                np -= RoomType.Small.GetMaxPeople();
                minPrice += SmallPrice;
            } 
            else if (RoomsMedium - nMedium > 0)
            {
                nMedium++;
                np -= RoomType.Medium.GetMaxPeople();
                minPrice += MediumPrice;
            }
            else if (RoomsLarge - nLarge > 0)
            {
                nLarge++;
                np -= RoomType.Large.GetMaxPeople();
                minPrice += LargePrice;
            }
            else if (RoomsApartment - nApartment > 0)
            {
                nApartment++;
                np -= RoomType.Apartment.GetMaxPeople();
                minPrice += ApartmentPrice;
            }
        }

        return minPrice * numberOfDays;
    }
}