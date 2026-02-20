// DTOs/ChatDtos.cs
namespace campus_insider.DTOs
{
    public class ChatConversationDto
    {
        public long Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<UserResponseDto> Participants { get; set; } = new();
        public ChatMessageDto? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ChatMessageDto
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public string Content { get; set; } = string.Empty;
        public UserResponseDto Sender { get; set; } = null!;
        public bool IsEdited { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? EditedAt { get; set; }
    }

    public class SendMessageDto
    {
        public string Content { get; set; } = string.Empty;
    }
}