using GeoNames.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;

namespace GeoNames.API
{
    public class GeoNamesClient : IGeoNamesClient
    {
        #region  Private Storage 

        static IEnumerable<Country> _countries;
        static IDictionary<string, int> _countryCodeMap;

        static Logger _logger = LogManager.GetCurrentClassLogger();

        string GeoNamesUserName;
        static Verbosity defaultVerbosity = Verbosity.medium;
        Verbosity verbosity;

        #endregion

        #region Constructors 

        public GeoNamesClient(string userName) : this(userName, defaultVerbosity)
        {
        }

        public GeoNamesClient(string userName, Verbosity verbosity)
        {
            GeoNamesUserName = userName;
            this.verbosity = verbosity;
        }

        #endregion

        #region Public APIs

        IGeoNamesClient IGeoNamesClient.Source { get; set; }

        public LookupResult LookupLocation(float latitude, float longitude)
        {
            var result = new LookupResult { StreetAddress = FindNearestAddress(latitude, longitude) };

            if (result.StreetAddress != null)
            {
                result.CountryCode = result.StreetAddress.countryCode;
                result.RegionCode = result.StreetAddress.regionCode;
                result.PostalCode = result.StreetAddress.postalcode;
            }
            else
            {
                result.Place = FindNearbyPlaces(latitude, longitude, 10, 1).First();

                result.CountryCode = result.Place.countryCode;
                result.RegionCode = result.Place.regionCode;
            }

            result.Regions = GetRegions(result.CountryCode);
            result.Cities = GetCities(result.CountryCode, result.RegionCode, verbosity);

            result.CityName = result.Cities.
                Where(city => city.Contains(latitude, longitude)).
                Select(city => city.cityName).
                FirstOrDefault();

            return result;
        }

        public IEnumerable<Country> GetCountries()
        {
            try
            {
                if (_countries == null)
                {
                    _countries = Countries(verbosity);
                    _countryCodeMap = _countries.Select(country => new
                        {
                            Code = country.countryCode,
                            GeoId = country.geonameId
                        }).ToDictionary(
                            ci => ci.Code, 
                            ci => ci.GeoId
                        );
                }

                return _countries;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Enumerable.Empty<Country>();
            }
        }

        public IEnumerable<Region> GetRegions(string countryCode, Verbosity style)
        {
            if (_countryCodeMap == null)
            {
                GetCountries();
            }

            var countryId = _countryCodeMap[countryCode];
            var regions = Children<Region>(countryId, style);
            return regions;
        }

        public IEnumerable<Region> GetRegions(string countryCode) =>
            GetRegions(countryCode, verbosity);

        public IEnumerable<City> GetCities(string countryCode, string regionCode, Verbosity style)
        {
            var cities = new List<City>();
            var startRow = 0;
            var maxRows = 1000;
            var urlParms = new[]
            {
                new KeyValuePair<string, string>("startRow", startRow.ToString()),
                new KeyValuePair<string, string>("country", countryCode),
                new KeyValuePair<string, string>("adminCode1", regionCode),
                new KeyValuePair<string, string>("cities", "cities1000"),
                new KeyValuePair<string, string>("maxRows", maxRows.ToString())
            };

            var result = Search<City>(urlParms, style);
            var totalResultsCount = result.Count();

            while (cities.Count < totalResultsCount)
            {
                cities.AddRange(result);

                startRow = cities.Count;
                urlParms[0] = new KeyValuePair<string, string>("startRow", startRow.ToString());

                result = Search<City>(urlParms, style);
            }

            return cities.OrderBy(city => city.cityName).ToList();
        }

        public IEnumerable<City> GetCities(string countryCode, string regionCode) =>
            GetCities(countryCode, regionCode, verbosity);

        public StreetAddress FindNearestAddress(float latitude, float longitude, Verbosity style) =>
            NearestAddress(latitude, longitude, style);

        public StreetAddress FindNearestAddress(float latitude, float longitude) =>
            FindNearestAddress(latitude, longitude, verbosity);

        public IEnumerable<Place> FindNearbyPlaces(float latitude, float longitude, float radius, int maxRows, Verbosity style) =>
            NearbyPlaces(latitude, longitude, radius, maxRows, style);

        public IEnumerable<Place> FindNearbyPlaces(float latitude, float longitude, float radius, int maxRows) =>
            FindNearbyPlaces(latitude, longitude, radius, maxRows, verbosity);

        public IEnumerable<PostalCode> PostalCodeLookup(string countryCode, string postalCode, int maxRows, Verbosity style) =>
            LookupPostalCode(countryCode, postalCode, maxRows, style);

        public IEnumerable<PostalCode> PostalCodeLookup(string countryCode, string postalCode, int maxRows) =>
            PostalCodeLookup(countryCode, postalCode, maxRows, verbosity);

        public Toponym Get(int geoNameId, Verbosity style) =>
            GetToponym(geoNameId, style);

        public Toponym Get(int geoNameId) =>
            Get(geoNameId, verbosity);

        public IEnumerable<Toponym> Hierarchy(int geoNameId, Verbosity style) =>
            GetHierarchy(geoNameId, style);

        public IEnumerable<Toponym> Hierarchy(int geoNameId) =>
            Hierarchy(geoNameId, verbosity);

        #endregion

        #region  Private Methods 

        Toponym HttpGet(string restUrl, Verbosity style, IEnumerable<KeyValuePair<string, string>> parms)
        {
            try
            {
                RestClient client = new RestClient("http://api.geonames.org");
                RestRequest request = new RestRequest(restUrl, Method.GET);
                request.AddQueryParameter("style", style.ToString())
                    .AddQueryParameter("username", GeoNamesUserName);

                if (parms != null)
                {
                    foreach (var parm in parms)
                    {
                        request.AddQueryParameter(parm.Key, parm.Value);
                    }
                }

                var response = client.Execute<Toponym>(request);

                var responseUri = response.ResponseUri.AbsoluteUri;
                _logger.Info($"Invoked Url {responseUri}");

                if (response.IsSuccessful)
                {
                    return response.Data;
                }

                _logger.Warn($"Url {responseUri} failed: {response.Content}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        IEnumerable<TToponym> GetWebServiceItems<ToponymList, TToponym>(string restUrl, Verbosity style, IEnumerable<KeyValuePair<string, string>> parms)
            where ToponymList : class, IToponymList<TToponym>, new()
            where TToponym : class, new()
        {
            try
            {
                RestClient client = new RestClient("http://api.geonames.org");
                RestRequest request = new RestRequest(restUrl, Method.GET);
                request.AddQueryParameter("style", style.ToString())
                    .AddQueryParameter("username", GeoNamesUserName);

                if (parms != null)
                {
                    foreach (var parm in parms)
                    {
                        request.AddQueryParameter(parm.Key, parm.Value);
                    }
                }

                var response = client.Execute<ToponymList>(request);

                var responseUri = response.ResponseUri.AbsoluteUri;
                _logger.Info($"Invoked Url {responseUri}");

                if (response.IsSuccessful)
                {
                    return response.Data.items;
                }

                _logger.Warn($"Url {responseUri} failed: {response.Content}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        TToponym GetWebServiceItem<ToponymWrapper, TToponym>(string restUrl, Verbosity style, IEnumerable<KeyValuePair<string, string>> parms)
            where ToponymWrapper : class, IToponymWrapper<TToponym>, new()
            where TToponym : class, new()
        {
            try
            {
                RestClient client = new RestClient("http://api.geonames.org");
                RestRequest request = new RestRequest(restUrl, Method.GET);
                request.AddQueryParameter("style", style.ToString())
                    .AddQueryParameter("username", GeoNamesUserName);

                if (parms != null)
                {
                    foreach (var parm in parms)
                    {
                        request.AddQueryParameter(parm.Key, parm.Value);
                    }
                }

                var response = client.Execute<ToponymWrapper>(request);

                var responseUri = response.ResponseUri.AbsoluteUri;
                _logger.Info($"Invoked Url {responseUri}");

                if (response.IsSuccessful)
                {
                    return response.Data.item;
                }

                _logger.Warn($"Url {responseUri} failed: {response.Content}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        IEnumerable<Country> Countries(Verbosity style) =>
            GetWebServiceItems<CountryList, Country>("/countryInfoJSON", style, Enumerable.Empty<KeyValuePair<string, string>>());

        IEnumerable<TToponym> Children<TToponym>(int geoNameId, Verbosity style) where TToponym : Toponym, new()
        {
            var parms = new[] { new KeyValuePair<string, string>("geonameId", geoNameId.ToString()) };
            return GetWebServiceItems<ToponymList<TToponym>, TToponym>("/childrenJSON", style, parms);
        }

        IEnumerable<TToponym> Search<TToponym>(IEnumerable<KeyValuePair<string, string>> parms, Verbosity style)
            where TToponym : Toponym, new() =>
            GetWebServiceItems<ToponymList<TToponym>, TToponym>("/searchJSON", style, parms);

        StreetAddress NearestAddress(float latitude, float longitude, Verbosity style)
        {
            var parms = new[]
            {
                new KeyValuePair<string, string>("lat", latitude.ToString()),
                new KeyValuePair<string, string>("lng", longitude.ToString()),
                new KeyValuePair<string, string>("radius", "1")
            };

            return GetWebServiceItem<StreetAddressLookupResult, StreetAddress>("/findNearestAddressJSON", style, parms);
        }

        IEnumerable<Place> NearbyPlaces(float latitude, float longitude, float radius, int maxRows, Verbosity style)
        {
            var parms = new[]
            {
                new KeyValuePair<string, string>("lat", latitude.ToString()),
                new KeyValuePair<string, string>("lng", longitude.ToString()),
                new KeyValuePair<string, string>("radius", radius.ToString()),
                new KeyValuePair<string, string>("maxRows", maxRows.ToString())
            };

            return GetWebServiceItems<PlaceList, Place>("/findNearbyPlaceNameJSON", style, parms);
        }

        IEnumerable<PostalCode> LookupPostalCode(string countryCode, string postalCode, int maxRows, Verbosity style) 
        {
            var parms = new[]
            {
                new KeyValuePair<string, string>("country", countryCode),
                new KeyValuePair<string, string>("postalcode", postalCode),
                new KeyValuePair<string, string>("maxRows", maxRows.ToString())
            };

            return GetWebServiceItems<PostalCodeList, PostalCode>("/postalCodeLookupJSON", style, parms);
        }

        Toponym GetToponym(int geoNameId, Verbosity style) 
        {
            var parms = new[] { new KeyValuePair<string, string>("geonameId", geoNameId.ToString()) };
            return HttpGet("/getJSON", style, parms);
        }

        IEnumerable<Toponym> GetHierarchy(int geoNameId, Verbosity style)
        {
            var parms = new[] { new KeyValuePair<string, string>("geonameId", geoNameId.ToString()) };
            return GetWebServiceItems<ToponymList, Toponym>("/hierarchyJSON", style, parms);
        }

        #endregion
    }
}