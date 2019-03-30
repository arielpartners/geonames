using System.Collections.Generic;

namespace GeoNames.Entities
{
    public class CountryListItem
    {
        public string countryCode { get; set; }
        public string countryName { get; set; }
    }

    public class RegionListItem
    {
        public string regionCode { get; set; }
        public string regionName { get; set; }
    }

    public class CityListItem
    {
        public string cityName { get; set; }
    }

    public class APILookupResult
    {
        public string streetAddress { get; set; }
        public string cityName { get; set; }
        public string regionCode { get; set; }
        public string countryCode { get; set; }
        public string postalCode { get; set; }

        public IEnumerable<CityListItem> cities { get; set; }
        public IEnumerable<RegionListItem> regions { get; set; }
    }
}
