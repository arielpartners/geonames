using System;
using System.IO;
using System.Linq;
using GeoNames.API;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeoNames.Tests
{
    [TestClass]
    public class TestApi
    {
        [TestMethod]
        public void GetCountries()
        {
            var client = GetClient();
            var countries = client.GetCountries();

            Assert.IsTrue(countries.Count() > 0);
        }

        [TestMethod]
        public void LookupLocation()
        {
            var client = GetClient();
            var location = client.LookupLocation(40.68607F, -73.921068F);

            Assert.IsNotNull(location);
        }

        [TestMethod]
        public void GetRegions()
        {
            var client = GetClient();
            var regions = client.GetRegions("US");

            Assert.IsTrue(regions.Count() > 0);
        }

        [TestMethod]
        public void GetCities()
        {
            var client = GetClient();
            var cities = client.GetCities("US", "NY");

            Assert.IsTrue(cities.Count() > 0);
        }

        [TestMethod]
        public void FindNearestAddress()
        {
            var client = GetClient();
            var address = client.FindNearestAddress(40.68607F, -73.921068F);

            Assert.IsNotNull(address);
        }

        [TestMethod]
        public void FindNearbyPlaces()
        {
            var client = GetClient();
            var places = client.FindNearbyPlaces(40.68607F, -73.921068F, 10F, 20);

            Assert.IsTrue(places.Count() > 0);
        }

        [TestMethod]
        public void PostalCodeLookup()
        {
            var client = GetClient();
            var postalCodes = client.PostalCodeLookup("US", "11216", 20);

            Assert.IsTrue(postalCodes.Count() > 0);
        }

        [TestMethod]
        public void Get()
        {
            var client = GetClient();
            var toponym = client.Get(6252001);

            Assert.IsNotNull(toponym);
        }

        [TestMethod]
        public void Hierarchy()
        {
            var client = GetClient();
            var hierarchy = client.Hierarchy(6252001);

            Assert.IsTrue(hierarchy.Count() > 0);
        }

        IGeoNamesClient GetClient()
        {

            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration["ConnectionStrings:GeoNamesCacheStore"];
            var geoNamesUsername = configuration["AppSettings:GeoNamesUserName"];
            var client = new BufferedClient(connectionString, geoNamesUsername);
            return client;
        }

    }
}
