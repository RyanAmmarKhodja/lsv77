// Models/Post.cs
namespace campus_insider.Models
{
    public class Post
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }

        // Content
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }

        // State
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User Author { get; set; } = null!;
    }

}