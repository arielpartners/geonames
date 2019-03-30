using System;
using System.Collections.Generic;
using System.Linq;
using GeoNames.Entities;
using NLog;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace GeoNames.API
{
    public class DBCacheClient : IGeoNamesClient
    {
        #region  Private Storage 

        static Logger _logger = LogManager.GetCurrentClassLogger();

        static readonly int expirationSeconds = 60 * 60 * 24 * 365;
        readonly string ConnectionString;
        static Verbosity defaultVerbosity = Verbosity.@short;
        readonly Verbosity verbosity;

        #endregion

        #region Constructors 

        public DBCacheClient(string connectionString, IGeoNamesClient source) : this(connectionString, defaultVerbosity, source)
        {
        }

        public DBCacheClient(string connectionString, Verbosity verbosity, IGeoNamesClient source)
        {
            ConnectionString = connectionString;
            this.verbosity = verbosity;
            Source = source;
        }

        #endregion

        #region Public APIs

        public IGeoNamesClient Source { get; set; }

        public IEnumerable<Country> GetCountries() => DBCache(Source.GetCountries, "GetCountries");

        public IEnumerable<City> GetCities(string countryCode, string regionCode, Verbosity style) =>
            DBCache(() => Source.GetCities(countryCode, regionCode, style), "GetCities", countryCode, regionCode, style);

        public IEnumerable<City> GetCities(string countryCode, string regionCode) => GetCities(countryCode, regionCode, verbosity);

        public IEnumerable<Region> GetRegions(string countryCode, Verbosity style) =>
            DBCache(() => Source.GetRegions(countryCode, style), "GetRegions", countryCode, style);

        public IEnumerable<Region> GetRegions(string countryCode) => GetRegions(countryCode, verbosity);

        public IEnumerable<Place> FindNearbyPlaces(float latitude, float longitude, float radius, int maxRows, Verbosity style) =>
            DBCache(() => Source.FindNearbyPlaces(latitude, longitude, radius, maxRows, style), "FindNearbyPlaces", latitude, longitude, radius, maxRows, style);

        public IEnumerable<Place> FindNearbyPlaces(float latitude, float longitude, float radius, int maxRows) =>
            FindNearbyPlaces(latitude, longitude, radius, maxRows, verbosity);

        public StreetAddress FindNearestAddress(float latitude, float longitude, Verbosity style) =>
            DBCache(() => Source.FindNearestAddress(latitude, longitude, style), "FindNearestAddress", latitude, longitude, style);

        public StreetAddress FindNearestAddress(float latitude, float longitude) =>
            FindNearestAddress(latitude, longitude, verbosity);

        public Toponym Get(int geoNameId, Verbosity style) => DBCache(() => Source.Get(geoNameId, style), "Get", geoNameId, style);

        public Toponym Get(int geoNameId) => Get(geoNameId, verbosity);

        public IEnumerable<Toponym> Hierarchy(int geoNameId, Verbosity style) =>
            DBCache(() => Source.Hierarchy(geoNameId, style), "Hierarchy", geoNameId, style);

        public IEnumerable<Toponym> Hierarchy(int geoNameId) => Hierarchy(geoNameId, verbosity);

        public LookupResult LookupLocation(float latitude, float longitude) => DBCache(() => Source.LookupLocation(latitude, longitude), "LookupLocation", latitude, longitude);

        public IEnumerable<PostalCode> PostalCodeLookup(string countryCode, string postalCode, int maxRows, Verbosity style) =>
            DBCache(() => Source.PostalCodeLookup(countryCode, postalCode, maxRows, style), "PostalCodeLookup", countryCode, postalCode, maxRows, style);

        public IEnumerable<PostalCode> PostalCodeLookup(string countryCode, string postalCode, int maxRows) =>
            PostalCodeLookup(countryCode, postalCode, maxRows, verbosity);

        #endregion

        string GetValue(IDbConnection db, string key) => db.Query<string>("GetValue", new { key }, commandType: CommandType.StoredProcedure).SingleOrDefault();
        void SetValue(IDbConnection db, string key, string value, DateTime expirationDate) => db.Execute("SetValue", new { key, value, expirationDate }, commandType: CommandType.StoredProcedure);

        R DBCache<R>(Func<R> sourceGetter, params object[] parms) where R : class
        {
            var key = GenerateCacheKey(parms);

            using (var db = new SqlConnection(ConnectionString))
            {
                R item = null;
                var serializedItem = GetValue(db, key);

                if (string.IsNullOrWhiteSpace(serializedItem))
                {
                    item = sourceGetter();

                    serializedItem = JsonConvert.SerializeObject(item);
                    var expirationDate = DateTime.UtcNow.AddSeconds(expirationSeconds);
                    SetValue(db, key, serializedItem, expirationDate);
                }
                else
                {
                    item = JsonConvert.DeserializeObject<R>(serializedItem);
                }

                return item;
            }
        }

        IEnumerable<R> DBCache<R>(Func<IEnumerable<R>> sourceGetter, params object[] parms)
        {
            var key = GenerateCacheKey(parms);

            using (var db = new SqlConnection(ConnectionString))
            {
                IEnumerable<R> items = null;
                var serializedItems = GetValue(db, key);

                if (string.IsNullOrWhiteSpace(serializedItems))
                {
                    items = sourceGetter();

                    serializedItems = JsonConvert.SerializeObject(items);
                    var expirationDate = DateTime.UtcNow.AddSeconds(expirationSeconds);
                    SetValue(db, key, serializedItems, expirationDate);
                }
                else
                {
                    items = JsonConvert.DeserializeObject<IEnumerable<R>>(serializedItems);
                }

                return items;
            }
        }

        static string GenerateCacheKey(params object[] parms)
        {
            if (parms == null)
            {
                throw new Exception("Cannot generate Cache Key with null parameter list!");
            }
            return string.Join(":", parms.Select(parm => (parm ?? string.Empty).ToString()));
        }
    }
}