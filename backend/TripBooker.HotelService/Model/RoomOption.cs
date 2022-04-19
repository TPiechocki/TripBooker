using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TripBooker.Common.Hotel;

namespace TripBooker.HotelService.Model;

internal class RoomOption
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public RoomType RoomType { get; set; }

    public double PriceModifier { get; set; }

    [Required]
    public string HotelCode { get; set; } = null!;

    public HotelOption Hotel { get; set; } = null!;
}
