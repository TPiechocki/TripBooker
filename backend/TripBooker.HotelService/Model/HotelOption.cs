using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TripBooker.HotelService.Model;

internal class HotelOption
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    [Required]
    public string City { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int Rating { get; set; }

    public double PriceStudio { get; set; }

    public double PriceSmall { get; set; }

    public double PriceMedium { get; set; }

    public double PriceLarge { get; set; }

    public double PriceApartment { get; set; }
}
