using Newtonsoft.Json;

namespace GeoNames.Entities
{
    public class ToponymBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string toponymName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double lat { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double lng { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int geonameId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fcl { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fclName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fcode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fcodeName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string continentCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string countryCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string countryName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string asciiName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string adminCode2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string adminName2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string adminCode3 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string adminName3 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string adminCode4 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string adminName4 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string adminCode5 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string adminName5 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public GeoTimeZone timezone { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? population { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? elevation { get; set; }

   }
}
