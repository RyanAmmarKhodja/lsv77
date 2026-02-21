namespace campus_insider.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalPosts { get; set; }
        public int ActivePosts { get; set; }
        public int TotalRidesConfirmed { get; set; }
        public int TotalEquipmentLoaned { get; set; }
        public int TotalConversations { get; set; }
        public int TotalMessages { get; set; }
        public List<CategoryStatDto> CategoryDistribution { get; set; } = new();
        public List<TopPostDto> MostViewedPosts { get; set; } = new();
        public List<DailyStatDto> PostsPerDay { get; set; } = new();
    }

    public class CategoryStatDto
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TopPostDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class DailyStatDto
    {
        public string Date { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
