// Models/ChatConversation.cs
namespace campus_insider.Models
{
    public class ChatConversation
    {
        public long Id { get; set; }
        public string Type { get; set; } = "DIRECT"; // DIRECT or GROUP
        public string? Name { get; set; } // For group chats
        public DateTime CreatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }

        // Navigation
        public List<ChatParticipant> Participants { get; set; } = new();
        public List<ChatMessage> Messages { get; set; } = new();
    }

    public class ChatParticipant
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long UserId { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LastReadAt { get; set; } // For unread count
        public bool IsMuted { get; set; } = false;

        // Navigation
        public ChatConversation Conversation { get; set; } = null!;
        public User User { get; set; } = null!;
    }

    public class ChatMessage
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? AttachmentUrl { get; set; } // For images/files
        public bool IsEdited { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? EditedAt { get; set; }

        // Navigation
        public ChatConversation Conversation { get; set; } = null!;
        public User Sender { get; set; } = null!;
        public List<ChatMessageRead> ReadReceipts { get; set; } = new();
    }

    public class ChatMessageRead
    {
        public long Id { get; set; }
        public long MessageId { get; set; }
        public long UserId { get; set; }
        public DateTime ReadAt { get; set; }

        // Navigation
        public ChatMessage Message { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}