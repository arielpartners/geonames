using NLog;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GeoNames.API
{
    [Produces("application/json")]
    [Route("api/geography")]
    public class GeographyController : ControllerBase
    {
        static protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        public GeographyController(IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:GeoNamesCacheStore"];
            var geoNamesUsername = configuration["AppSettings:GeoNamesUserName"];
            GeoNamesManager.Initialize(connectionString, geoNamesUsername);
        }

        [HttpGet]
        [Route("countries", Name = "Get Countries")]
        public IActionResult GetCountries()
        {
            try
            {
                // Create new buffered client.
                var client = new BufferedClient();
                var countries = client.GetCountries();
                return Ok(countries);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("regions/{countryCode}", Name = "Get regions in country")]
        public IActionResult PostRegions(string countryCode)
        {
            try
            {
                // If the input was invalid, throw a 400 error.
                if (string.IsNullOrWhiteSpace(countryCode))
                {
                    return BadRequest(ModelState);
                }

                // Create new buffered client.
                var client = new BufferedClient();
                var regions = client.GetRegions(countryCode);
                return Ok(regions);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("cities/{countryCode}/{adminCode1}", Name = "Get cities in country/region")]
        public IActionResult PostCities(string countryCode, string adminCode1)
        {
            try
            {
                // If the input was invalid, throw a 400 error.
                if (string.IsNullOrWhiteSpace(countryCode))
                {
                    return BadRequest(ModelState);
                }

                // Create new buffered client.
                var client = new BufferedClient();
                var cities = client.GetCities(countryCode, adminCode1);
                return Ok(cities);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return BadRequest(ex);
            }
        }

        void LogError(Exception ex)
        {
            ex = ex.GetBaseException();
            logger.Error(ex, ex.Message);
        }
    }
}
