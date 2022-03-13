using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TripBooker.Common.Transport
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransportType
    {
        Flight,
        Private
    }
}