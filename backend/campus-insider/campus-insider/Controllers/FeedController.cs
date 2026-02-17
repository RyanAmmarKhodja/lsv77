using campus_insider.DTOs;
using campus_insider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace campus_insider.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/feed")]
    public class FeedController : ControllerBase
    {
        private readonly FeedService _feedService;

        public FeedController(FeedService feedService)
        {
            _feedService = feedService;
        }

        private long GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(userIdString, out long userId) ? userId : 0;
        }


        // GET: api/feed
        // Query params: ?type=coride&page=1&pageSize=20
        [HttpGet]
        public async Task<IActionResult> GetFeed(
            [FromQuery] string? type = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(new { message = "Invalid pagination parameters" });

            var posts = await _feedService.GetFeedAsync(type, page, pageSize);

            return Ok(new
            {
                page,
                pageSize,
                data = posts
            });
        }

        // GET: api/feed/search
        // Query params: ?q=paris&type=coride
        [HttpGet("search")]
        public async Task<IActionResult> SearchPosts(
            [FromQuery] string q,
            [FromQuery] string? type = null)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest(new { message = "Search term is required" });

            var posts = await _feedService.SearchPostsAsync(q, type);
            return Ok(posts);
        }

        // GET: api/feed/corides/upcoming
        // Query params: ?location=paris
        [HttpGet("corides/upcoming")]
        public async Task<IActionResult> GetUpcomingCorides([FromQuery] string? location = null)
        {
            var corides = await _feedService.GetUpcomingCoridesAsync(location);
            return Ok(corides);
        }

        // GET: api/feed/equipment/available
        // Query params: ?category=tools&location=paris
        [HttpGet("equipment/available")]
        public async Task<IActionResult> GetAvailableEquipment(
            [FromQuery] string? category = null,
            [FromQuery] string? location = null)
        {
            var equipment = await _feedService.GetAvailableEquipmentAsync(category, location);
            return Ok(equipment);
        }
    }
}