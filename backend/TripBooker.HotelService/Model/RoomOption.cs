using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TripBooker.Common.Hotel;

namespace TripBooker.HotelService.Model;

public class RoomOption
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [ForeignKey("HotelOption")]
    public int HotelID { get; set; }

    public RoomType RoomType { get; set; }

    public double PriceModifier { get; set; }
}
