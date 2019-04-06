using System.Collections.Generic;
using GeoNames.Entities;
using NLog;

namespace GeoNames.API
{
    public class MemCacheClient : IGeoNamesClient
    {
        #region  Private Storage 

        static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        static readonly int GeoCacheTimeoutSeconds = 600;// int.Parse(ConfigurationManager.AppSettings["GeoCacheTimeoutSeconds"]);
        readonly Verbosity verbosity;

        #endregion

        #region Constructors 

        public MemCacheClient(Verbosity verbosity, IGeoNamesClient source)
        {
            this.verbosity = verbosity;
            Source = source;
        }

        #endregion

        #region Public APIs

        public string SourceName() => "Memory Cache Client";

        public IGeoNamesClient Source { get; set; }

        public IApiMetricResult<IEnumerable<Place>> FindNearbyPlaces(float latitude, float longitude, float radius, int maxRows, Verbosity style)
        {
            var places = SimpleCache.Get(
                () => Source.FindNearbyPlaces(latitude, longitude, radius, maxRows, style),
                GeoCacheTimeoutSeconds, "MemCacheClient.FindNearbyPlaces", latitude, longitude, radius, maxRows, style
                );

            return places.Value;
        }

        public IApiMetricResult<IEnumerable<Place>> FindNearbyPlaces(float latitude, float longitude, float radius, int maxRows) =>
            FindNearbyPlaces(latitude, longitude, radius, maxRows, verbosity);

        public IApiMetricResult<StreetAddress> FindNearestAddress(float latitude, float longitude, Verbosity style)
        {
            var address = SimpleCache.Get(
                () => Source.FindNearestAddress(latitude, longitude, style),
                GeoCacheTimeoutSeconds, "MemCacheClient.FindNearestAddress", latitude, longitude, style
                );

            return address.Value;
        }

        public IApiMetricResult<StreetAddress> FindNearestAddress(float latitude, float longitude) => FindNearestAddress(latitude, longitude, verbosity);

        public IApiMetricResult<Toponym> Get(int geoNameId, Verbosity style)
        {
            var top = SimpleCache.Get(
                () => Source.Get(geoNameId, style),
                GeoCacheTimeoutSeconds, "MemCacheClient.Get", geoNameId, style
                );

            return top.Value;
        }

        public IApiMetricResult<Toponym> Get(int geoNameId) => Get(geoNameId, verbosity);

        public IApiMetricResult<IEnumerable<City>> GetCities(string countryCode, string regionCode, Verbosity style)
        {
            var cities = SimpleCache.Get(
                () => Source.GetCities(countryCode, regionCode, style),
                GeoCacheTimeoutSeconds, "MemCacheClient.GetCities", countryCode, regionCode, style
                );

            return cities.Value;
        }

        public IApiMetricResult<IEnumerable<City>> GetCities(string countryCode, string regionCode) =>
            GetCities(countryCode, regionCode, verbosity);

        public IApiMetricResult<IEnumerable<Country>> GetCountries()
        {
            var countries = SimpleCache.Get(
                Source.GetCountries,
                GeoCacheTimeoutSeconds, "MemCacheClient.GetCountries"
                );

            return countries.Value;
        }

        public IApiMetricResult<IEnumerable<Region>> GetRegions(string countryCode, Verbosity style)
        {
            var regions = SimpleCache.Get(
                () => Source.GetRegions(countryCode, style),
                GeoCacheTimeoutSeconds, "MemCacheClient.GetRegions", countryCode, style
                );

            return regions.Value;
        }

        public IApiMetricResult<IEnumerable<Region>> GetRegions(string countryCode) => GetRegions(countryCode, verbosity);

        public IApiMetricResult<IEnumerable<Toponym>> Hierarchy(int geoNameId, Verbosity style)
        {
            var hier = SimpleCache.Get(
                () => Source.Hierarchy(geoNameId, style),
                GeoCacheTimeoutSeconds, "MemCacheClient.Hierarchy", geoNameId, style
                );

            return hier.Value;
        }

        public IApiMetricResult<IEnumerable<Toponym>> Hierarchy(int geoNameId) => Hierarchy(geoNameId, verbosity);

        public IApiMetricResult<LookupResult> LookupLocation(float latitude, float longitude)
        {
            var location = SimpleCache.Get(
                () => Source.LookupLocation(latitude, longitude),
                GeoCacheTimeoutSeconds, "MemCacheClient.LookupLocation", latitude, longitude
                );

            return location.Value;
        }

        public IApiMetricResult<IEnumerable<PostalCode>> PostalCodeLookup(string countryCode, string postalCode, int maxRows, Verbosity style)
        {
            var location = SimpleCache.Get(
                () => Source.PostalCodeLookup(countryCode, postalCode, maxRows, style),
                GeoCacheTimeoutSeconds, "MemCacheClient.PostalCodeLookup", countryCode, postalCode, maxRows, style
                );

            return location.Value;
        }

        public IApiMetricResult<IEnumerable<PostalCode>> PostalCodeLookup(string countryCode, string postalCode, int maxRows) =>
            PostalCodeLookup(countryCode, postalCode, maxRows, verbosity);

        #endregion
    }
}
