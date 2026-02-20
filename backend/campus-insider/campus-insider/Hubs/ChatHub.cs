// Hubs/ChatHub.cs
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using campus_insider.Services;

namespace campus_insider.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ChatService _chatService;

        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                // Add user to their personal group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");

                // Add user to all their conversation groups
                var conversationIds = await _chatService.GetUserConversationIds(long.Parse(userId));
                foreach (var convId in conversationIds)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation-{convId}");
                }
            }
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(long conversationId, string content)
        {
            var userId = long.Parse(Context.User!.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            var result = await _chatService.SendMessage(conversationId, userId, content);
            if (result.Success)
            {
                // Broadcast to conversation group
                await Clients.Group($"conversation-{conversationId}")
                    .SendAsync("ReceiveMessage", result.Data);
            }
        }

        public async Task TypingIndicator(long conversationId, bool isTyping)
        {
            var userId = Context.User!.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;

            await Clients.OthersInGroup($"conversation-{conversationId}")
                .SendAsync("UserTyping", new { userId, isTyping });
        }

        public async Task MarkAsRead(long conversationId)
        {
            var userId = long.Parse(Context.User!.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            await _chatService.MarkConversationAsRead(conversationId, userId);
        }
    }
}