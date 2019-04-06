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

        static IApiMetricResult<IEnumerable<Country>> _countries;
        static IDictionary<string, int> _countryCodeMap;

        static Logger _logger = LogManager.GetCurrentClassLogger();
        readonly string GeoNamesUserName;
        static Verbosity defaultVerbosity = Verbosity.medium;
        readonly Verbosity verbosity;

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

        public string SourceName() => "GeoNames.org Web Service Client";

        IGeoNamesClient IGeoNamesClient.Source { get; set; }

        public IApiMetricResult<LookupResult> LookupLocation(float latitude, float longitude)
        {
            var result = new ApiMetricResult<LookupResult>
            {
                ResultSource = SourceName(),
                Value = new LookupResult()
            };
            var lookupResult = result.Value;

            var streetAddress = FindNearestAddress(latitude, longitude);

            if (streetAddress.Value != null)
            {
                result.CreatedTime = streetAddress.CreatedTime;
                result.TimeToCreate = streetAddress.TimeToCreate;
                lookupResult.CountryCode = streetAddress.Value.countryCode;
                lookupResult.RegionCode = streetAddress.Value.regionCode;
                lookupResult.PostalCode = streetAddress.Value.postalcode;
            }
            else
            {
                var places = FindNearbyPlaces(latitude, longitude, 10, 1);
                result.CreatedTime = places.CreatedTime;
                result.TimeToCreate = places.TimeToCreate;

                var place = places.Value.First();
                lookupResult.CountryCode = place.countryCode;
                lookupResult.RegionCode = place.regionCode;
            }

            var regions = GetRegions(lookupResult.CountryCode);
            result.TimeToCreate += regions.TimeToCreate;
            lookupResult.Regions = regions.Value;

            var cities = GetCities(lookupResult.CountryCode, lookupResult.RegionCode, verbosity);
            result.TimeToCreate += cities.TimeToCreate;
            lookupResult.Cities = cities.Value;

            lookupResult.CityName = lookupResult.Cities.
                Where(city => city.Contains(latitude, longitude)).
                Select(city => city.cityName).
                FirstOrDefault();

            return result;
        }

        public IApiMetricResult<IEnumerable<Country>> GetCountries()
        {
            try
            {
                if (_countries == null)
                {
                    _countries = Countries(verbosity);
                    _countryCodeMap = _countries.Value.Select(country => new
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
                return new ApiMetricResult<IEnumerable<Country>>
                {
                    ResultSource = SourceName(),
                    Value = Enumerable.Empty<Country>()
                };
            }
        }

        public IApiMetricResult<IEnumerable<Region>> GetRegions(string countryCode, Verbosity style)
        {
            if (_countryCodeMap == null)
            {
                GetCountries();
            }

            var countryId = _countryCodeMap[countryCode];
            var regions = Children<Region>(countryId, style);
            return regions;
        }

        public IApiMetricResult<IEnumerable<Region>> GetRegions(string countryCode) =>
            GetRegions(countryCode, verbosity);

        public IApiMetricResult<IEnumerable<City>> GetCities(string countryCode, string regionCode, Verbosity style)
        {
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

            var _cities = Search<City>(urlParms, style);
            var totalResultsCount = _cities.Value.Count();

            var result = new ApiMetricResult<IEnumerable<City>>
            {
                ResultSource = SourceName(),
                CreatedTime = _cities.CreatedTime,
                Value = new List<City>()
            };
            var cities = result.Value as List<City>;

            while (cities.Count < totalResultsCount)
            {
                cities.AddRange(_cities.Value);
                result.TimeToCreate += _cities.TimeToCreate;

                startRow = cities.Count;
                urlParms[0] = new KeyValuePair<string, string>("startRow", startRow.ToString());

                _cities = Search<City>(urlParms, style);
            }

            result.Value = cities.OrderBy(city => city.cityName).ToList();
            return result;
        }

        public IApiMetricResult<IEnumerable<City>> GetCities(string countryCode, string regionCode) =>
            GetCities(countryCode, regionCode, verbosity);

        public IApiMetricResult<StreetAddress> FindNearestAddress(float latitude, float longitude, Verbosity style) =>
            NearestAddress(latitude, longitude, style);

        public IApiMetricResult<StreetAddress> FindNearestAddress(float latitude, float longitude) =>
            FindNearestAddress(latitude, longitude, verbosity);

        public IApiMetricResult<IEnumerable<Place>> FindNearbyPlaces(float latitude, float longitude, float radius, int maxRows, Verbosity style) =>
            NearbyPlaces(latitude, longitude, radius, maxRows, style);

        public IApiMetricResult<IEnumerable<Place>> FindNearbyPlaces(float latitude, float longitude, float radius, int maxRows) =>
            FindNearbyPlaces(latitude, longitude, radius, maxRows, verbosity);

        public IApiMetricResult<IEnumerable<PostalCode>> PostalCodeLookup(string countryCode, string postalCode, int maxRows, Verbosity style) =>
            LookupPostalCode(countryCode, postalCode, maxRows, style);

        public IApiMetricResult<IEnumerable<PostalCode>> PostalCodeLookup(string countryCode, string postalCode, int maxRows) =>
            PostalCodeLookup(countryCode, postalCode, maxRows, verbosity);

        public IApiMetricResult<Toponym> Get(int geoNameId, Verbosity style) =>
            GetToponym(geoNameId, style);

        public IApiMetricResult<Toponym> Get(int geoNameId) =>
            Get(geoNameId, verbosity);

        public IApiMetricResult<IEnumerable<Toponym>> Hierarchy(int geoNameId, Verbosity style) =>
            GetHierarchy(geoNameId, style);

        public IApiMetricResult<IEnumerable<Toponym>> Hierarchy(int geoNameId) =>
            Hierarchy(geoNameId, verbosity);

        #endregion

        #region  Private Methods 

        IRestResponse<T> HttpGet<T>(string restUrl, Verbosity style, IEnumerable<KeyValuePair<string, string>> parms)
            where T : class, new()
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

                var response = client.Execute<T>(request);
                var responseUri = response.ResponseUri.AbsoluteUri;
                _logger.Info($"Invoked Url {responseUri}");

                if (response.IsSuccessful)
                {
                    return response;
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

        IApiMetricResult<Toponym> HttpGet(string restUrl, Verbosity style, IEnumerable<KeyValuePair<string, string>> parms) =>
            new ApiMetricResult<Toponym>(SourceName(), () =>
        {
            var response = HttpGet<Toponym>(restUrl, style, parms);
            return response?.Data;
        });

        IApiMetricResult<IEnumerable<TToponym>> GetWebServiceItems<ToponymList, TToponym>(string restUrl, Verbosity style, IEnumerable<KeyValuePair<string, string>> parms)
            where ToponymList : class, IToponymList<TToponym>, new()
            where TToponym : class, new() =>
            new ApiMetricResult<IEnumerable<TToponym>>(SourceName(), () =>
        {
            var response = HttpGet<ToponymList>(restUrl, style, parms);
            return response?.Data.items;
        });

        IApiMetricResult<TToponym> GetWebServiceItem<ToponymWrapper, TToponym>(string restUrl, Verbosity style, IEnumerable<KeyValuePair<string, string>> parms)
            where ToponymWrapper : class, IToponymWrapper<TToponym>, new()
            where TToponym : class, new() =>
            new ApiMetricResult<TToponym>(SourceName(), () =>
        {
            var response = HttpGet<ToponymWrapper>(restUrl, style, parms);
            return response?.Data.item;
        });

        IApiMetricResult<IEnumerable<Country>> Countries(Verbosity style) =>
            GetWebServiceItems<CountryList, Country>("/countryInfoJSON", style, Enumerable.Empty<KeyValuePair<string, string>>());

        IApiMetricResult<IEnumerable<TToponym>> Children<TToponym>(int geoNameId, Verbosity style) where TToponym : Toponym, new()
        {
            var parms = new[] { new KeyValuePair<string, string>("geonameId", geoNameId.ToString()) };
            return GetWebServiceItems<ToponymList<TToponym>, TToponym>("/childrenJSON", style, parms);
        }

        IApiMetricResult<IEnumerable<TToponym>> Search<TToponym>(IEnumerable<KeyValuePair<string, string>> parms, Verbosity style)
            where TToponym : Toponym, new() =>
            GetWebServiceItems<ToponymList<TToponym>, TToponym>("/searchJSON", style, parms);

        IApiMetricResult<StreetAddress> NearestAddress(float latitude, float longitude, Verbosity style)
        {
            var parms = new[]
            {
                new KeyValuePair<string, string>("lat", latitude.ToString()),
                new KeyValuePair<string, string>("lng", longitude.ToString()),
                new KeyValuePair<string, string>("radius", "1")
            };

            return GetWebServiceItem<StreetAddressLookupResult, StreetAddress>("/findNearestAddressJSON", style, parms);
        }

        IApiMetricResult<IEnumerable<Place>> NearbyPlaces(float latitude, float longitude, float radius, int maxRows, Verbosity style)
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

        IApiMetricResult<IEnumerable<PostalCode>> LookupPostalCode(string countryCode, string postalCode, int maxRows, Verbosity style) 
        {
            var parms = new[]
            {
                new KeyValuePair<string, string>("country", countryCode),
                new KeyValuePair<string, string>("postalcode", postalCode),
                new KeyValuePair<string, string>("maxRows", maxRows.ToString())
            };

            return GetWebServiceItems<PostalCodeList, PostalCode>("/postalCodeLookupJSON", style, parms);
        }

        IApiMetricResult<Toponym> GetToponym(int geoNameId, Verbosity style) 
        {
            var parms = new[] { new KeyValuePair<string, string>("geonameId", geoNameId.ToString()) };
            return HttpGet("/getJSON", style, parms);
        }

        IApiMetricResult<IEnumerable<Toponym>> GetHierarchy(int geoNameId, Verbosity style)
        {
            var parms = new[] { new KeyValuePair<string, string>("geonameId", geoNameId.ToString()) };
            return GetWebServiceItems<ToponymList, Toponym>("/hierarchyJSON", style, parms);
        }

        #endregion
    }
}
