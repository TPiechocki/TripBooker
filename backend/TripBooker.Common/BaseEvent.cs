using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TripBooker.Common;

[Index(nameof(StreamId), nameof(Version), IsUnique = true)]
public class BaseEvent
{
    public BaseEvent(Guid streamId, int version, string type, object data)
    {
        StreamId = streamId;
        Version = version;
        Type = type;
        Data = JsonConvert.SerializeObject(data);
    }

    public BaseEvent(Guid streamId, int version, string type, string data)
    {
        StreamId = streamId;
        Version = version;
        Type = type;
        Data = data;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public Guid StreamId { get; set; }

    [Required]
    public int Version { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }

    [Required] 
    public string Type { get; set; }

    public string Data { get; set; }
}