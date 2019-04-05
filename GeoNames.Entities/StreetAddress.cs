using Newtonsoft.Json;

namespace GeoNames.Entities
{
    public class StreetAddress : PostalCode
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string street { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string mtfcc { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string streetNumber { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float distance { get; set; }

        public override string ToString()
        {
            return $"{streetNumber} {street} {placeName}, {regionCode} {postalcode}";
        }
    }

    public class StreetAddressLookupResult : IToponymWrapper<StreetAddress>
    {
        public StreetAddress address { get; set; }
        StreetAddress IToponymWrapper<StreetAddress>.item { get => address; set => address = value; }
    }
}
