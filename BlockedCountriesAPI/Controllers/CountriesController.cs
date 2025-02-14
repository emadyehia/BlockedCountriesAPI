using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace BlockedCountriesAPI.Controllers
{
    [Route("api/countries")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        public static readonly ConcurrentDictionary<string, bool> BlockedCountries = new();

        [HttpPost("block")]
        public IActionResult BlockCountry([FromBody] string countryCode)
        {
            if (BlockedCountries.ContainsKey(countryCode))
                return Conflict("Country is already blocked.");

            BlockedCountries[countryCode] = true;
            return Ok($"Country {countryCode} blocked.");
        }

        [HttpDelete("block/{countryCode}")]
        public IActionResult UnblockCountry(string countryCode)
        {
            if (!BlockedCountries.TryRemove(countryCode, out _))
                return NotFound("Country not found in blocked list.");

            return Ok($"Country {countryCode} unblocked.");
        }

        [HttpGet("blocked")]
        public IActionResult GetBlockedCountries()
        {
            return Ok(BlockedCountries.Keys);
        }
    }
}
