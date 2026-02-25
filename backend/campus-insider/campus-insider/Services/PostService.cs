using campus_insider.Data;
using campus_insider.DTOs;
using campus_insider.Models;
using Microsoft.EntityFrameworkCore;

namespace campus_insider.Services
{
    public class PostService : IPostService
    {
        private readonly AppDbContext _context;
        private readonly NotificationService _notificationService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<PostService> _logger;

        public PostService(AppDbContext context, NotificationService notificationService, IServiceScopeFactory serviceScopeFactory, ILogger<PostService> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<CorideDto> CreateCorideAsync(long authorId, CreateCorideDto dto)
        {
            var coride = new Coride
            {
                AuthorId = authorId,
                Title = dto.Title,
                Content = dto.Content,
                Category = Category.COVOITURAGE,
                PostType = dto.PostType,
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

            var corideId = coride.Id;
            var corideTitle = coride.Title;

            // Notify all users about the new post
            try
            {
                await _notificationService.BroadcastNotificationToAllUsers(
                    "NEW_POST",
                    "Nouvelle annonce",
                    $"Nouvelle annonce covoiturage : {coride.Title}",
                    sendEmail: false,
                    actionUrl: $"/post/{coride.Id}",
                    actionText: "Voir l'annonce"
                );
            }
            catch
            {
                // Log error if needed, but allow the method to return the DTO
            }

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
                PostType = dto.PostType,
                ImageUrl = dto.ImageUrl,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Equipment.Add(equipment);
            await _context.SaveChangesAsync();

            var equipmentId = equipment.Id;
            var equipmentTitle = equipment.Title;


            try
            {

                await _notificationService.BroadcastNotificationToAllUsers(
                    "NEW_POST",
                    "Nouvelle annonce",
                    $"Nouvelle annonce : {equipment.Title}",
                    sendEmail: false,
                    actionUrl: $"/post/{equipment.Id}",
                    actionText: "Voir l'annonce"
                );
            }
            catch (Exception ex) { _logger.LogError(ex, "Failed to broadcast notification for equipment {EquipmentId}", equipmentId); }


            return await MapToEquipmentDto(equipment);
        }

        public async Task<PostDto?> GetPostByIdAsync(long id)
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return null;

            // Increment view count
            post.ViewCount++;
            await _context.SaveChangesAsync();

            return post switch
            {
                Coride coride => await MapToCorideDto(coride),
                Equipment equipment => await MapToEquipmentDto(equipment),
                _ => null
            };
        }

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
                Author = coride.Author != null ? new UserResponseDto
                {
                    Id = coride.Author.Id,
                    FirstName = coride.Author.FirstName,
                    LastName = coride.Author.LastName,
                    Email = coride.Author.Email,
                    Role = coride.Author.Role,
                    CreatedAt = coride.Author.CreatedAt
                } : null,
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
                Author = equipment.Author != null ? new UserResponseDto
                {
                    Id = equipment.Author.Id,
                    FirstName = equipment.Author.FirstName,
                    LastName = equipment.Author.LastName,
                    Email = equipment.Author.Email,
                    Role = equipment.Author.Role,
                    CreatedAt = equipment.Author.CreatedAt
                } : null,
                Title = equipment.Title,
                Content = equipment.Content,
                ImageUrl = equipment.ImageUrl,
                IsActive = equipment.IsActive,
                ViewCount = equipment.ViewCount,
                CreatedAt = equipment.CreatedAt,
                Category = equipment.Category,
                PostType = equipment.PostType,
                Location = equipment.Location,
            };
        }
    }
}