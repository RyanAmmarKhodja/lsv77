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
        public string ImagePublicId { get; set; } = string.Empty;
        public Category Category { get; set; } = Category.AUTRE;
        public PostType PostType { get; set; } = PostType.OFFER;

        // State
        public bool IsActive { get; set; } = true;
        public int ViewCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User Author { get; set; } = null!;
    }

}