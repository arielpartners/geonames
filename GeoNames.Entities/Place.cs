using Newtonsoft.Json;
using System.Collections.Generic;

namespace GeoNames.Entities
{
    public class Place : ToponymBase
    {
        [JsonProperty("adminCode1", NullValueHandling = NullValueHandling.Ignore)]
        public string regionCode { get; set; }

        [JsonProperty("adminName1", NullValueHandling = NullValueHandling.Ignore)]
        public string regionName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float distance { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int countryId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string adminId1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string adminId2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string adminId3 { get; set; }

        public override string ToString()
        {
            return name;
        }
    }

    public class PlaceList : IToponymList<Place>
    {
        public IEnumerable<Place> geonames { get; set; }
        IEnumerable<Place> IToponymList<Place>.items { get => geonames; set => geonames = value; }
    }
}
