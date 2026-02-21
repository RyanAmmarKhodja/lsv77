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
        private readonly NotificationService _notificationService;

        public ChatHub(ChatService chatService, NotificationService notificationService)
        {
            _chatService = chatService;
            _notificationService = notificationService;
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

                // Notify other participants about the new message
                var recipientIds = await _chatService.GetOtherParticipantIds(conversationId, userId);
                foreach (var recipientId in recipientIds)
                {
                    try
                    {
                        await _notificationService.CreateNotification(
                            recipientId,
                            "NEW_MESSAGE",
                            "Nouveau message",
                            $"{result.Data!.Sender.FirstName}: {(content.Length > 50 ? content[..50] + "..." : content)}",
                            sendEmail: false,
                            entityType: "ChatConversation",
                            entityId: conversationId,
                            actionUrl: $"/chat",
                            actionText: "Voir le message"
                        );
                    }
                    catch { /* Don't fail message send if notification fails */ }
                }
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