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
        var price = HotelConstants.BaseRoomPrice
               * PriceModifier
               * Rooms.Where(r => r.RoomType == roomType)
                      .FirstOrDefault(new RoomOption { PriceModifier = 0.0 }).PriceModifier;

        return Math.Round(price, 2);
    }

    public double GetPriceFor(MealOption mealOption)
    {
        var price = mealOption switch
        {
            MealOption.AllInclusive => HotelConstants.BaseAllInclusivePrice * PriceModifier,
            MealOption.ContinentalBreakfeast => HotelConstants.BaseBreakfeastPrice * PriceModifier,
            _ => 0
        };

        return Math.Round(price, 2);
    }
}