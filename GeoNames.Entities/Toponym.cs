using Newtonsoft.Json;
using System.Collections.Generic;

namespace GeoNames.Entities
{
    public class Toponym : ToponymBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<alternateName> alternateNames { get; set; } = new List<alternateName>();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? numberOfChildren { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string adminCode1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string adminName1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int srtm3 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string wikipediaURL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public BoundingBox bbox { get; set; }

        public bool Contains(float latitude, float longitude)
        {
            return bbox != null && bbox.Contains(latitude, longitude);
        }

        public override string ToString()
        {
            return toponymName;
        }

        public class alternateName
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string name { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string lang { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public bool? isShortName { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public bool? isPreferredName { get; set; }

            public override string ToString()
            {
                return $"{lang}:{name}";
            }
        }

        public class BoundingBox
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public float? accuracyLevel { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public float? west { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public float? north { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public float? east { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public float? south { get; set; }

            public bool Contains(float latitude, float longitude)
            {
                return BoundariesDefined() && IsBetween(latitude, north, south) && IsBetween(longitude, east, west);
            }

            bool BoundariesDefined()
            {
                return north.HasValue && south.HasValue && east.HasValue && west.HasValue;
            }

            static bool IsBetween(float num, float? num1, float? num2)
            {
                if (num1 < num2)
                {
                    return num1.Value <= num && num <= num2.Value;
                }
                return num1.Value >= num && num >= num2.Value;
            }
        }
    }

    public class ToponymList : IToponymList<Toponym>
    {
        public IEnumerable<Toponym> geonames { get; set; }
        IEnumerable<Toponym> IToponymList<Toponym>.items { get => geonames; set => geonames = value; }
    }

    public class ToponymList<TToponymClass> : IToponymList<TToponymClass>
    {
        public IEnumerable<TToponymClass> geonames { get; set; }
        public int totalResultsCount { get; set; }
        IEnumerable<TToponymClass> IToponymList<TToponymClass>.items { get => geonames; set => geonames = value; }
    }
}
