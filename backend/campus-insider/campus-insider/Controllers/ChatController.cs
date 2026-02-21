// Controllers/ChatController.cs
using campus_insider.DTOs;
using campus_insider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace campus_insider.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        private long GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(userIdString, out long userId) ? userId : 0;
        }

        // GET /api/chat/conversations
        [HttpGet("conversations")]
        public async Task<ActionResult<List<ChatConversationDto>>> GetConversations()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var conversations = await _chatService.GetUserConversations(userId);
            return Ok(conversations);
        }

        // POST /api/chat/conversations/direct/{otherUserId}?postId=123
        [HttpPost("conversations/direct/{otherUserId}")]
        public async Task<ActionResult<ChatConversationDto>> CreateDirectConversation(long otherUserId, [FromQuery] long? postId = null)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _chatService.CreateOrGetDirectConversation(userId, otherUserId);
            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            // Record post interaction if coming from a post page
            if (postId.HasValue)
            {
                await _chatService.RecordPostInteraction(postId.Value, userId);
            }

            return Ok(result.Data);
        }

        // GET /api/chat/conversations/5/messages?pageNumber=1&pageSize=50
        [HttpGet("conversations/{conversationId}/messages")]
        public async Task<ActionResult<PagedResult<ChatMessageDto>>> GetMessages(
            long conversationId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var messages = await _chatService.GetConversationMessages(conversationId, userId, pageNumber, pageSize);
            return Ok(messages);
        }

        // POST /api/chat/conversations/5/messages (HTTP endpoint, SignalR recommended)
        [HttpPost("conversations/{conversationId}/messages")]
        public async Task<ActionResult<ChatMessageDto>> SendMessage(
            long conversationId,
            [FromBody] SendMessageDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _chatService.SendMessage(conversationId, userId, dto.Content);
            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        // GET /api/chat/unread-count
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var count = await _chatService.GetUnreadCount(userId);
            return Ok(new { count });
        }
    }
}