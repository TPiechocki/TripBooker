using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TripBooker.Common.Hotel;

namespace TripBooker.HotelService.Model;

internal class HotelOption
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public string Code { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Country { get; set; } = null!;

    [Required]
    public string AirportCode { get; set; } = null!;

    public float Rating { get; set; }

    public double PriceModifier { get; set; }

    public bool AllInclusive { get; set; }

    public List<RoomOption> Rooms { get; set; } = new List<RoomOption>();

    public double GetPriceFor(RoomType roomType)
    {
        return HotelConstants.BaseRoomPrice
               * PriceModifier
               * Rooms.Where(r => r.RoomType == roomType)
                      .FirstOrDefault(new RoomOption { PriceModifier = 0.0 }).PriceModifier;
    }

    public double GetPriceFor(MealOption mealOption)
    {
        switch (mealOption)
        {
            case MealOption.AllInclusive:
                return HotelConstants.BaseAllInclusivePrice * PriceModifier;
            case MealOption.ContinentalBreakfeast:
                return HotelConstants.BaseBreakfeastPrice * PriceModifier;
            case MealOption.NoMeals:
            default:
                return 0;
        }
    }
}