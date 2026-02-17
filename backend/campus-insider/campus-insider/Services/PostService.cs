using campus_insider.Data;
using campus_insider.DTOs;
using campus_insider.Models;
using Microsoft.EntityFrameworkCore;

namespace campus_insider.Services
{
    public class PostService : IPostService
    {
        private readonly AppDbContext _context;

        public PostService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CorideDto> CreateCorideAsync(long authorId, CreateCorideDto dto)
        {
            var coride = new Coride
            {
                AuthorId = authorId,
                Title = dto.Title,
                Content = dto.Content,
                DepartureTime = dto.DepartureTime,
                DepartureLocation = dto.DepartureLocation,
                DestinationLocation = dto.DestinationLocation,
                AvailableSeats = dto.AvailableSeats,
                ReturnTime = dto.ReturnTime,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Corides.Add(coride);
            await _context.SaveChangesAsync();

            return await MapToCorideDto(coride);
        }

        public async Task<EquipmentDto> CreateEquipmentAsync(long authorId, CreateEquipmentDto dto)
        {
            var equipment = new Equipment
            {
                AuthorId = authorId,
                Title = dto.Title,
                Content = dto.Content,
                Location = dto.Location,
                Category = dto.Category,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Equipment.Add(equipment);
            await _context.SaveChangesAsync();

            return await MapToEquipmentDto(equipment);
        }

        public async Task<PostDto?> GetPostByIdAsync(long id)
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return null;

            return post switch
            {
                Coride coride => await MapToCorideDto(coride),
                Equipment equipment => await MapToEquipmentDto(equipment),
                _ => null
            };
        }

        //public async Task<IEnumerable<PostDto>> GetUserPostsAsync(long userId)
        //{
        //    var posts = await _context.Posts
        //        .Include(p => p.Author)
        //        .Where(p => p.AuthorId == userId)
        //        .OrderByDescending(p => p.CreatedAt)
        //        .ToListAsync();

        //    return posts.Select(post => post switch
        //    {
        //        Coride coride => MapToCorideDto(coride).Result,
        //        Equipment equipment => MapToEquipmentDto(equipment).Result,
        //        _ => null
        //    }).Where(dto => dto != null).Cast<PostDto>();
        //}

        public async Task<bool> DeletePostAsync(long id, long authorId)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id && p.AuthorId == authorId);

            if (post == null) return false;

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivatePostAsync(long id, long authorId)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id && p.AuthorId == authorId);

            if (post == null) return false;

            post.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        // Helper mapping methods
        private async Task<CorideDto> MapToCorideDto(Coride coride)
        {
            if (coride.Author == null)
            {
                await _context.Entry(coride).Reference(c => c.Author).LoadAsync();
            }

            return new CorideDto
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
            };
        }

        private async Task<EquipmentDto> MapToEquipmentDto(Equipment equipment)
        {
            if (equipment.Author == null)
            {
                await _context.Entry(equipment).Reference(e => e.Author).LoadAsync();
            }

            return new EquipmentDto
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
            };
        }
    }
}