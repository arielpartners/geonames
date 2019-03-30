using GeoNames.Entities;
using System.Collections.Generic;

namespace GeoNames.API
{
    public interface IGeoNamesClient
    {
        LookupResult LookupLocation(float latitude, float longitude);
        IEnumerable<Country> GetCountries();
        IEnumerable<Region> GetRegions(string countryCode, Verbosity style);
        IEnumerable<Region> GetRegions(string countryCode);
        IEnumerable<City> GetCities(string countryCode, string regionCode, Verbosity style);
        IEnumerable<City> GetCities(string countryCode, string regionCode);
        StreetAddress FindNearestAddress(float latitude, float longitude, Verbosity style);
        StreetAddress FindNearestAddress(float latitude, float longitude);
        IEnumerable<Place> FindNearbyPlaces(float latitude, float longitude, float radius, int maxRows, Verbosity style);
        IEnumerable<Place> FindNearbyPlaces(float latitude, float longitude, float radius, int maxRows);
        IEnumerable<PostalCode> PostalCodeLookup(string countryCode, string postalCode, int maxRows, Verbosity style);
        IEnumerable<PostalCode> PostalCodeLookup(string countryCode, string postalCode, int maxRows);
        Toponym Get(int geoNameId, Verbosity style);
        Toponym Get(int geoNameId);
        IEnumerable<Toponym> Hierarchy(int geoNameId, Verbosity style);
        IEnumerable<Toponym> Hierarchy(int geoNameId);
        IGeoNamesClient Source { get; set; }
    }
}
