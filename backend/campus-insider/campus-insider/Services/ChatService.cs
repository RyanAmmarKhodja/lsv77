// Services/ChatService.cs
using campus_insider.Data;
using campus_insider.DTOs;
using campus_insider.Models;
using Microsoft.EntityFrameworkCore;

namespace campus_insider.Services
{
    public class ChatService
    {
        private readonly AppDbContext _context;

        public ChatService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<ChatConversationDto>> CreateOrGetDirectConversation(long user1Id, long user2Id)
        {
            // Check if conversation already exists
            var existing = await _context.ChatConversations
                .Include(c => c.Participants)
                .ThenInclude(p => p.User)
                .Where(c => c.Type == "DIRECT" && c.Participants.Count == 2)
                .Where(c => c.Participants.Any(p => p.UserId == user1Id) &&
                           c.Participants.Any(p => p.UserId == user2Id))
                .FirstOrDefaultAsync();

            if (existing != null)
                return ServiceResult<ChatConversationDto>.Ok(MapConversationToDto(existing, user1Id));

            // Create new conversation
            var conversation = new ChatConversation
            {
                Type = "DIRECT",
                CreatedAt = DateTime.UtcNow,
                Participants = new List<ChatParticipant>
                {
                    new ChatParticipant { UserId = user1Id, JoinedAt = DateTime.UtcNow },
                    new ChatParticipant { UserId = user2Id, JoinedAt = DateTime.UtcNow }
                }
            };

            _context.ChatConversations.Add(conversation);
            await _context.SaveChangesAsync();

            // Reload with includes
            var created = await _context.ChatConversations
                .Include(c => c.Participants)
                .ThenInclude(p => p.User)
                .FirstAsync(c => c.Id == conversation.Id);

            return ServiceResult<ChatConversationDto>.Ok(MapConversationToDto(created, user1Id));
        }

        public async Task<ServiceResult<ChatMessageDto>> SendMessage(long conversationId, long senderId, string content)
        {
            // Validate participant
            var isParticipant = await _context.ChatParticipants
                .AnyAsync(p => p.ConversationId == conversationId && p.UserId == senderId);

            if (!isParticipant)
                return ServiceResult<ChatMessageDto>.Fail("You are not a participant in this conversation.");

            if (string.IsNullOrWhiteSpace(content) || content.Length > 5000)
                return ServiceResult<ChatMessageDto>.Fail("Invalid message content.");

            var message = new ChatMessage
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Content = content.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(message);

            // Update conversation last message time
            var conversation = await _context.ChatConversations.FindAsync(conversationId);
            conversation!.LastMessageAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Reload with sender
            var sent = await _context.ChatMessages
                .Include(m => m.Sender)
                .FirstAsync(m => m.Id == message.Id);

            return ServiceResult<ChatMessageDto>.Ok(MapMessageToDto(sent));
        }

        public async Task<PagedResult<ChatMessageDto>> GetConversationMessages(
            long conversationId,
            long requestingUserId,
            int pageNumber = 1,
            int pageSize = 50)
        {
            // Verify user is participant
            var isParticipant = await _context.ChatParticipants
                .AnyAsync(p => p.ConversationId == conversationId && p.UserId == requestingUserId);

            if (!isParticipant)
                return new PagedResult<ChatMessageDto>();

            var query = _context.ChatMessages
                .AsNoTracking()
                .Include(m => m.Sender)
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ChatMessageDto>
            {
                Items = items.Select(MapMessageToDto).Reverse().ToList(), // Reverse to show oldest first
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<List<ChatConversationDto>> GetUserConversations(long userId)
        {
            var conversations = await _context.ChatConversations
                .AsNoTracking()
                .Include(c => c.Participants)
                .ThenInclude(p => p.User)
                .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .ThenInclude(m => m.Sender)
                .Where(c => c.Participants.Any(p => p.UserId == userId))
                .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                .ToListAsync();

            return conversations.Select(c => MapConversationToDto(c, userId)).ToList();
        }

        public async Task<List<long>> GetUserConversationIds(long userId)
        {
            return await _context.ChatParticipants
                .Where(p => p.UserId == userId)
                .Select(p => p.ConversationId)
                .ToListAsync();
        }

        public async Task MarkConversationAsRead(long conversationId, long userId)
        {
            var participant = await _context.ChatParticipants
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (participant != null)
            {
                participant.LastReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetUnreadCount(long userId)
        {
            var participants = await _context.ChatParticipants
                .Include(p => p.Conversation)
                .ThenInclude(c => c.Messages)
                .Where(p => p.UserId == userId)
                .ToListAsync();

            int unreadCount = 0;
            foreach (var participant in participants)
            {
                var lastRead = participant.LastReadAt ?? participant.JoinedAt;
                unreadCount += participant.Conversation.Messages
                    .Count(m => m.CreatedAt > lastRead && m.SenderId != userId && !m.IsDeleted);
            }

            return unreadCount;
        }

        public async Task<List<long>> GetOtherParticipantIds(long conversationId, long senderId)
        {
            return await _context.ChatParticipants
                .Where(p => p.ConversationId == conversationId && p.UserId != senderId)
                .Select(p => p.UserId)
                .ToListAsync();
        }

        private ChatConversationDto MapConversationToDto(ChatConversation conversation, long currentUserId)
        {
            var otherParticipant = conversation.Type == "DIRECT"
                ? conversation.Participants.FirstOrDefault(p => p.UserId != currentUserId)
                : null;

            var lastMessage = conversation.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault();

            return new ChatConversationDto
            {
                Id = conversation.Id,
                Type = conversation.Type,
                Name = conversation.Type == "DIRECT" && otherParticipant != null
                    ? $"{otherParticipant.User.FirstName} {otherParticipant.User.LastName}"
                    : conversation.Name ?? "Group Chat",
                Participants = conversation.Participants.Select(p => new UserResponseDto
                {
                    Id = p.User.Id,
                    FirstName = p.User.FirstName,
                    LastName = p.User.LastName,
                    Email = p.User.Email,
                    Role = p.User.Role,
                    CreatedAt = p.User.CreatedAt
                }).ToList(),
                LastMessage = lastMessage != null ? MapMessageToDto(lastMessage) : null,
                LastMessageAt = conversation.LastMessageAt,
                CreatedAt = conversation.CreatedAt
            };
        }

        private ChatMessageDto MapMessageToDto(ChatMessage message) => new()
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            Content = message.Content,
            Sender =  new UserResponseDto
            {
                Id = message.Sender.Id,
                FirstName = message.Sender.FirstName,
                LastName = message.Sender.LastName,
                Email = message.Sender.Email,
                Role = message.Sender.Role,
                CreatedAt = message.Sender.CreatedAt
            },
            IsEdited = message.IsEdited,
            CreatedAt = message.CreatedAt,
            EditedAt = message.EditedAt
        };

        public async Task RecordPostInteraction(long postId, long userId)
        {
            var interaction = new PostInteraction
            {
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.PostInteractions.Add(interaction);
            await _context.SaveChangesAsync();
        }
    }
}