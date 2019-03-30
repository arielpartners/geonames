using System.Collections.Generic;

namespace GeoNames.Entities
{
    public class LookupResult
    {
        public StreetAddress StreetAddress { get; set; }
        public Place Place { get; set; }

        public string CityName { get; set; }
        public string RegionCode { get; set; }
        public string CountryCode { get; set; }
        public string PostalCode { get; set; }

        public IEnumerable<City> Cities { get; set; }
        public IEnumerable<Region> Regions { get; set; }
    }
}
