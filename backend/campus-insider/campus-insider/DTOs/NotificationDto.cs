namespace campus_insider.DTOs
{
    public class NotificationDto
    {
        public long Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? EntityType { get; set; }
        public long? EntityId { get; set; }
        public string? ActionUrl { get; set; }
        public string? ActionText { get; set; }
    }
}