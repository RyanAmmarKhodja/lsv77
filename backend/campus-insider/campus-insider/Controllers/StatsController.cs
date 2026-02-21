using campus_insider.DTOs;
using campus_insider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace campus_insider.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/stats")]
    public class StatsController : ControllerBase
    {
        private readonly StatsService _statsService;
        private const long AdminUserId = 15;

        public StatsController(StatsService statsService)
        {
            _statsService = statsService;
        }

        private long GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(userIdString, out long userId) ? userId : 0;
        }

        // GET /api/stats/dashboard
        [Authorize(Roles="ADMIN")]

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            var userId = GetCurrentUserId();
            var stats = await _statsService.GetDashboardStatsAsync();
            return Ok(stats);
        }
    }
}
