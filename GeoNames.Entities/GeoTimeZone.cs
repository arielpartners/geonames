using Newtonsoft.Json;

namespace GeoNames.Entities
{
    public class GeoTimeZone
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string timeZoneId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float dstOffset { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float gmtOffset { get; set; }
    }
}
