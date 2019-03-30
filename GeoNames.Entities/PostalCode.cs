using Newtonsoft.Json;
using System.Collections.Generic;

namespace GeoNames.Entities
{
    public class PostalCode
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string postalcode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string placeName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string countryCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double lat { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double lng { get; set; }

        [JsonProperty("adminCode1", NullValueHandling = NullValueHandling.Ignore)]
        public string regionCode { get; set; }

        [JsonProperty("adminName1", NullValueHandling = NullValueHandling.Ignore)]
        public string regionName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string adminCode2 { get; set; }

        [JsonProperty("adminName2", NullValueHandling = NullValueHandling.Ignore)]
        public string countyName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string adminCode3 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string adminName3 { get; set; }

        public override string ToString()
        {
            return postalcode;
        }
    }

    public class PostalCodeList : IToponymList<PostalCode>
    {
        public IEnumerable<PostalCode> postalcodes { get; set; }
        IEnumerable<PostalCode> IToponymList<PostalCode>.items { get => postalcodes; set => postalcodes = value; }
    }
}
