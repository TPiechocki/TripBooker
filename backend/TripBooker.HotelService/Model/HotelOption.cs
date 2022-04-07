using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TripBooker.HotelService.Model;

internal class HotelOption
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    [Required]
    public string City { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int Rating { get; set; }

    public double PriceModifier { get; set; }

    public List<RoomOption> Rooms { get; set; } = new List<RoomOption>();
}
