using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountriesAPI.Controllers
{
    [Route("api/logs")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        [HttpGet("blocked-attempts")]
        public IActionResult GetBlockedAttempts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var logs = IPController.BlockedLogs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(logs);
        }
    }
}
