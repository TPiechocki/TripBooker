using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    public string City { get; set; } = null!;

    [Required]
    public string AirportCode { get; set; } = null!;

    public int Rating { get; set; }

    public double PriceModifier { get; set; }

    public bool AllInclusive { get; set; }

    public List<RoomOption> Rooms { get; set; } = new List<RoomOption>();
}
