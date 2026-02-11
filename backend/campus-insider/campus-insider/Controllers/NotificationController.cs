using campus_insider.DTOs;
using campus_insider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace campus_insider.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private long GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(userIdString, out long userId) ? userId : 0;
        }

        // GET /api/notifications?isRead=false&pageNumber=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<PagedResult<NotificationDto>>> GetNotifications(
            [FromQuery] bool? isRead = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _notificationService.GetUserNotifications(userId, isRead, pageNumber, pageSize);
            return Ok(result);
        }

        // GET /api/notifications/unread-count
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var count = await _notificationService.GetUnreadCount(userId);
            return Ok(new { count });
        }

        // PATCH /api/notifications/5/read
        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _notificationService.MarkAsRead(id, userId);
            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { message = "Notification marked as read." });
        }

        // PATCH /api/notifications/read-all
        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            await _notificationService.MarkAllAsRead(userId);
            return Ok(new { message = "All notifications marked as read." });
        }

        // DELETE /api/notifications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _notificationService.DeleteNotification(id, userId);
            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { message = "Notification deleted." });
        }
    }
}