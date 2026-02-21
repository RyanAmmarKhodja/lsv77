using campus_insider.Data;
using campus_insider.DTOs;
using campus_insider.Models;
using Microsoft.EntityFrameworkCore;

namespace campus_insider.Services
{
    public class FeedService : IFeedService
    {
        private readonly AppDbContext _context;

        public FeedService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PostDto>> GetFeedAsync(string? category, string? type = null, int page = 1, int pageSize = 20)
        {
            IQueryable<Post> query = _context.Posts
                .Include(p => p.Author)
                .Where(p => p.IsActive);

            // Filter by PostType (OFFER / DEMAND)
            if (!string.IsNullOrEmpty(type) && Enum.TryParse<PostType>(type, true, out var postType))
            {
                query = query.Where(p => p.PostType == postType);
            }

            // Filter by Category enum
            if (!string.IsNullOrEmpty(category) && Enum.TryParse<Category>(category, true, out var cat))
            {
                query = query.Where(p => p.Category == cat);
            }

            // Pagination
            var posts = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

            return posts.Select(MapToDto).Where(dto => dto != null).Cast<PostDto>();
        }

        public async Task<IEnumerable<PostDto>> SearchPostsAsync(string searchTerm, string? type = null)
        {
            IQueryable<Post> query = _context.Posts
                .Include(p => p.Author)
                .Where(p => p.IsActive);

            // Filter by PostType if specified
            if (!string.IsNullOrEmpty(type) && Enum.TryParse<PostType>(type, true, out var postType))
            {
                query = query.Where(p => p.PostType == postType);
            }

            // Search in title and description
            var posts = await query
                .Where(p => p.Title.Contains(searchTerm) || p.Content.Contains(searchTerm))
                .OrderByDescending(p => p.CreatedAt)
                .Take(50) // Limit search results
                .ToListAsync();

            return posts.Select(MapToDto).Where(dto => dto != null).Cast<PostDto>();
        }

        public async Task<IEnumerable<CorideDto>> GetUpcomingCoridesAsync(string? location = null)
        {
            var query = _context.Corides
                .Include(c => c.Author)
                .Where(c => c.IsActive && c.DepartureTime > DateTime.UtcNow);

            // Filter by location if specified
            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(c =>
                    c.DepartureLocation.Contains(location) ||
                    c.DestinationLocation.Contains(location));
            }

            var corides = await query
                .OrderBy(c => c.DepartureTime)
                .Take(50)
                .ToListAsync();

            return corides.Select(c => MapToDto(c) as CorideDto).Where(dto => dto != null).Cast<CorideDto>();
        }

        public async Task<IEnumerable<EquipmentDto>> GetAvailableEquipmentAsync(string? category = null, string? location = null)
        {
            var query = _context.Equipment
                .Include(e => e.Author)
                .Where(e => e.IsActive);

            // Filter by category if specified
            if (!string.IsNullOrEmpty(category) && Enum.TryParse<Category>(category, true, out var cat))
            {
                query = query.Where(e => e.Category == cat);
            }

            // Filter by location if specified
            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(e => e.Location.Contains(location));
            }

            var equipment = await query
                .OrderByDescending(e => e.CreatedAt)
                .Take(50)
                .ToListAsync();

            return equipment.Select(e => MapToDto(e) as EquipmentDto).Where(dto => dto != null).Cast<EquipmentDto>();
        }

        // Helper mapping method
        private PostDto? MapToDto(Post post)
        {
            var authorDto = post.Author != null ? new UserResponseDto
            {
                Id = post.Author.Id,
                FirstName = post.Author.FirstName,
                LastName = post.Author.LastName,
                Email = post.Author.Email,
                Role = post.Author.Role,
                CreatedAt = post.Author.CreatedAt
            } : null;

            return post switch
            {
                Coride coride => new CorideDto
                {
                    Id = coride.Id,
                    AuthorId = coride.AuthorId,
                    Author = authorDto,
                    Title = coride.Title,
                    Content = coride.Content,
                    IsActive = coride.IsActive,
                    ViewCount = coride.ViewCount,
                    CreatedAt = coride.CreatedAt,
                    PostType = coride.PostType,
                    Category = coride.Category,
                    DepartureTime = coride.DepartureTime,
                    DepartureLocation = coride.DepartureLocation,
                    DestinationLocation = coride.DestinationLocation,
                    AvailableSeats = coride.AvailableSeats,
                    ReturnTime = coride.ReturnTime,
                },
                Equipment equipment => new EquipmentDto
                {
                    Id = equipment.Id,
                    AuthorId = equipment.AuthorId,
                    Author = authorDto,
                    Title = equipment.Title,
                    Content = equipment.Content,
                    ImageUrl = equipment.ImageUrl,
                    IsActive = equipment.IsActive,
                    ViewCount = equipment.ViewCount,
                    CreatedAt = equipment.CreatedAt,
                    PostType = equipment.PostType,
                    Category = equipment.Category,
                    Location = equipment.Location,
                },
                _ => null
            };
        }
    }
}