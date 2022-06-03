using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TripBooker.TourOperator.Model;

internal class UpdateModel
{
    public UpdateModel(DateTime timestamp, string description)
    {
        Timestamp = timestamp;
        Description = description;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }

    public string Description { get; set; }
}
