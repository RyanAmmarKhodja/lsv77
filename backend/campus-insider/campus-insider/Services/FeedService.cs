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
    
            public async Task<IEnumerable<PostDto>> GetFeedAsync(string? type = null, int page = 1, int pageSize = 20)
            {
                IQueryable<Post> query = _context.Posts
                    .Include(p => p.Author)
                    .Where(p => p.IsActive);

                // Filter by type if specified
                if (!string.IsNullOrEmpty(type))
                {
                    query = type.ToLower() switch
                    {
                        "coride" => query.OfType<Coride>(),
                        "equipment" => query.OfType<Equipment>(),
                        _ => query
                    };
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

                // Filter by type if specified
                if (!string.IsNullOrEmpty(type))
                {
                    query = type.ToLower() switch
                    {
                        "coride" => query.OfType<Coride>(),
                        "equipment" => query.OfType<Equipment>(),
                        _ => query
                    };
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
                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(e => e.Category != null && e.Category.Contains(category));
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
                return post switch
                {
                    Coride coride => new CorideDto
                    {
                        Id = coride.Id,
                        AuthorId = coride.AuthorId,
                        Title = coride.Title,
                        Content = coride.Content,
                        IsActive = coride.IsActive,
                        CreatedAt = coride.CreatedAt,
                        PostType = "Coride",
                        DepartureTime = coride.DepartureTime,
                        DepartureLocation = coride.DepartureLocation,
                        DestinationLocation = coride.DestinationLocation,
                        AvailableSeats = coride.AvailableSeats,
                        ReturnTime = coride.ReturnTime
                    },
                    Equipment equipment => new EquipmentDto
                    {
                        Id = equipment.Id,
                        AuthorId = equipment.AuthorId,
                        Title = equipment.Title,
                        Content = equipment.Content,
                        IsActive = equipment.IsActive,
                        CreatedAt = equipment.CreatedAt,
                        PostType = "Equipment",
                        Location = equipment.Location,
                        Category = equipment.Category
                    },
                    _ => null
                };
            }
        }
}