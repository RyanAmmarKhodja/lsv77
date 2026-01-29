namespace campus_insider.Models
{
    public class Notification
    {
        public long Id { get; set; }

        public string Type { get; set; } = null!;
        public string Content { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        // Foreign key
        public long UserId { get; set; }

        // Navigation
        public User User { get; set; } = null!;
    }

}
