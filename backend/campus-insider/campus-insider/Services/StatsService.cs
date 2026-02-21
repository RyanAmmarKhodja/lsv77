using campus_insider.Data;
using campus_insider.DTOs;
using campus_insider.Models;
using Microsoft.EntityFrameworkCore;

namespace campus_insider.Services
{
    public class StatsService
    {
        private readonly AppDbContext _context;

        public StatsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var stats = new DashboardStatsDto();

            // Basic counts
            stats.TotalUsers = await _context.Users.CountAsync();
            stats.TotalPosts = await _context.Posts.CountAsync();
            stats.ActivePosts = await _context.Posts.CountAsync(p => p.IsActive);
            stats.TotalConversations = await _context.ChatConversations.CountAsync();
            stats.TotalMessages = await _context.ChatMessages.CountAsync(m => !m.IsDeleted);

            // Confirmed rides: post interactions on COVOITURAGE posts
            stats.TotalRidesConfirmed = await _context.PostInteractions
                .Include(pi => pi.Post)
                .CountAsync(pi => pi.Post.Category == Category.COVOITURAGE);

            // Equipment loaned: post interactions on non-COVOITURAGE posts
            stats.TotalEquipmentLoaned = await _context.PostInteractions
                .Include(pi => pi.Post)
                .CountAsync(pi => pi.Post.Category != Category.COVOITURAGE);

            // Category distribution
            stats.CategoryDistribution = await _context.Posts
                .GroupBy(p => p.Category)
                .Select(g => new CategoryStatDto
                {
                    Category = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToListAsync();

            // Most viewed posts (top 10)
            stats.MostViewedPosts = await _context.Posts
                .Include(p => p.Author)
                .Where(p => p.ViewCount > 0)
                .OrderByDescending(p => p.ViewCount)
                .Take(10)
                .Select(p => new TopPostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Category = p.Category.ToString(),
                    ViewCount = p.ViewCount,
                    AuthorName = p.Author.FirstName + " " + p.Author.LastName,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            // Posts per day (last 30 days)
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            stats.PostsPerDay = await _context.Posts
                .Where(p => p.CreatedAt >= thirtyDaysAgo)
                .GroupBy(p => p.CreatedAt.Date)
                .Select(g => new DailyStatDto
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Count = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToListAsync();

            return stats;
        }
    }
}
