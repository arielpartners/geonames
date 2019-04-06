using GeoNames.Entities;
using System.Collections.Generic;

namespace GeoNames.API
{
    public interface IGeoNamesClient
    {
        IApiMetricResult<LookupResult> LookupLocation(float latitude, float longitude);
        IApiMetricResult<IEnumerable<Country>> GetCountries();
        IApiMetricResult<IEnumerable<Region>> GetRegions(string countryCode, Verbosity style);
        IApiMetricResult<IEnumerable<Region>> GetRegions(string countryCode);
        IApiMetricResult<IEnumerable<City>> GetCities(string countryCode, string regionCode, Verbosity style);
        IApiMetricResult<IEnumerable<City>> GetCities(string countryCode, string regionCode);
        IApiMetricResult<StreetAddress> FindNearestAddress(float latitude, float longitude, Verbosity style);
        IApiMetricResult<StreetAddress> FindNearestAddress(float latitude, float longitude);
        IApiMetricResult<IEnumerable<Place>> FindNearbyPlaces(float latitude, float longitude, float radius, int maxRows, Verbosity style);
        IApiMetricResult<IEnumerable<Place>> FindNearbyPlaces(float latitude, float longitude, float radius, int maxRows);
        IApiMetricResult<IEnumerable<PostalCode>> PostalCodeLookup(string countryCode, string postalCode, int maxRows, Verbosity style);
        IApiMetricResult<IEnumerable<PostalCode>> PostalCodeLookup(string countryCode, string postalCode, int maxRows);
        IApiMetricResult<Toponym> Get(int geoNameId, Verbosity style);
        IApiMetricResult<Toponym> Get(int geoNameId);
        IApiMetricResult<IEnumerable<Toponym>> Hierarchy(int geoNameId, Verbosity style);
        IApiMetricResult<IEnumerable<Toponym>> Hierarchy(int geoNameId);
        IGeoNamesClient Source { get; set; }
        string SourceName();
    }
}
