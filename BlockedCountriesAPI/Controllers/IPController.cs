using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace BlockedCountriesAPI.Controllers
{
    [Route("api/ip")]
    [ApiController]
    public class IPController : ControllerBase
    {
        private readonly GeolocationService _geoService;
        public static readonly ConcurrentBag<BlockedAttemptLog> BlockedLogs = new();

        public IPController(GeolocationService geoService)
        {
            _geoService = geoService;
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> LookupIP([FromQuery] string ipAddress)
        {
            var countryCode = await _geoService.GetCountryCodeByIpAsync(ipAddress);
            return Ok(new { ipAddress, countryCode });
        }

        [HttpGet("check-block")]
        public async Task<IActionResult> CheckBlock()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();
            var countryCode = await _geoService.GetCountryCodeByIpAsync(ipAddress);

            bool isBlocked = CountriesController.BlockedCountries.ContainsKey(countryCode);

            BlockedLogs.Add(new BlockedAttemptLog
            {
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow,
                CountryCode = countryCode,
                IsBlocked = isBlocked,
                UserAgent = userAgent
            });

            return Ok(new { ipAddress, countryCode, isBlocked });
        }
    }
}
