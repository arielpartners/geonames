using Newtonsoft.Json;
using System.Collections.Generic;

namespace GeoNames.Entities
{
    public class Country
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string countryCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string countryName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? isoNumeric { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string isoAlpha3 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fipsCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string continent { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string continentName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string capital { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string areaInSqKm { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long population { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string currencyCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string languages { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int geonameId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float? west { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float? north { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float? east { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float? south { get; set; }

        // NOTE: This property appears to only get populated by the Xml api call.

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string postalCodeFormat { get; set; }

        public override string ToString()
        {
            return countryName;
        }

    }

    public class CountryList : IToponymList<Country>
    {
        public IEnumerable<Country> geonames { get; set; }
        IEnumerable<Country> IToponymList<Country>.items { get => geonames; set => geonames = value; }
    }
}
