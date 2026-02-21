namespace campus_insider.Models
{
    /// <summary>
    /// Tracks when a user clicks "Message poster" on a post.
    /// Used as a proxy for confirmed rides / equipment loans in statistics.
    /// </summary>
    public class PostInteraction
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Post Post { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
