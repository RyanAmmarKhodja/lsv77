using campus_insider.Data;
using campus_insider.DTOs;
using campus_insider.Models;
using Microsoft.EntityFrameworkCore;

namespace campus_insider.Services
{
    

    public class EquipmentService
    {
        private readonly AppDbContext _context;

        public EquipmentService(AppDbContext context)
        {
            _context = context;
        }

        #region --- Queries ---

        public async Task<PagedResult<EquipmentResponseDto>> GetAllEquipment(int pageNumber = 1, int pageSize = 20)
        {
            var query = _context.Equipment.AsNoTracking();

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(e => e.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<EquipmentResponseDto>
            {
                Items = items.Select(MapToResponseDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<EquipmentResponseDto?> GetByEquipmentIdAsync(long id)
        {
            var equipment = await _context.Equipment.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            return equipment != null ? MapToResponseDto(equipment) : null;
        }

        public async Task<PagedResult<EquipmentResponseDto>> GetEquipmentByOwner(
            long userId,
            int pageNumber = 1,
            int pageSize = 20)
        {
            var query = _context.Equipment.AsNoTracking().Where(e => e.OwnerId == userId);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(e => e.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<EquipmentResponseDto>
            {
                Items = items.Select(MapToResponseDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<EquipmentResponseDto>> SearchEquipment(
            string? searchTerm = null,
            string? category = null,
            int pageNumber = 1,
            int pageSize = 20)
        {
            var query = _context.Equipment.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(e =>
                    e.Name.ToLower().Contains(searchTerm) ||
                    e.Description.ToLower().Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(e => e.Category == category);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(e => e.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<EquipmentResponseDto>
            {
                Items = items.Select(MapToResponseDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        #endregion

        #region --- Commands ---

        public async Task<ServiceResult<EquipmentResponseDto>> ShareEquipment(EquipmentCreateDto dto, long ownerId)
        {
            // Validation 1: Name is required and reasonable length
            if (string.IsNullOrWhiteSpace(dto.Name))
                return ServiceResult<EquipmentResponseDto>.Fail("Equipment name is required.");

            if (dto.Name.Length > 200)
                return ServiceResult<EquipmentResponseDto>.Fail("Equipment name must be 200 characters or less.");

            // Validation 2: Category is required
            if (string.IsNullOrWhiteSpace(dto.Category))
                return ServiceResult<EquipmentResponseDto>.Fail("Category is required.");

            // Validation 3: Check for duplicate (same owner, same name)
            var exists = await _context.Equipment.AnyAsync(e =>
                e.OwnerId == ownerId &&
                e.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                return ServiceResult<EquipmentResponseDto>.Fail("You already have equipment with this name.");

            var equipment = new Equipment
            {
                Name = dto.Name.Trim(),
                Category = dto.Category.Trim(),
                Description = dto.Description?.Trim() ?? string.Empty,
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Equipment.Add(equipment);
            await _context.SaveChangesAsync();

            return ServiceResult<EquipmentResponseDto>.Ok(MapToResponseDto(equipment));
        }

        public async Task<ServiceResult> UpdateEquipment(long equipmentId, EquipmentUpdateDto dto, long requestingUserId)
        {
            var equipment = await _context.Equipment.FindAsync(equipmentId);
            if (equipment == null)
                return ServiceResult.Fail("Equipment not found.");

            // Authorization: Only owner can update
            if (equipment.OwnerId != requestingUserId)
                return ServiceResult.Fail("You are not authorized to update this equipment.");

            // Validation: Name is required
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                if (dto.Name.Length > 200)
                    return ServiceResult.Fail("Equipment name must be 200 characters or less.");

                // Check for duplicate name (excluding current equipment)
                var duplicateExists = await _context.Equipment.AnyAsync(e =>
                    e.OwnerId == requestingUserId &&
                    e.Id != equipmentId &&
                    e.Name.ToLower() == dto.Name.ToLower());

                if (duplicateExists)
                    return ServiceResult.Fail("You already have equipment with this name.");

                equipment.Name = dto.Name.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.Category))
                equipment.Category = dto.Category.Trim();

            if (dto.Description != null)
                equipment.Description = dto.Description.Trim();

            await _context.SaveChangesAsync();
            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> UnshareEquipment(long equipmentId, long requestingUserId)
        {
            var equipment = await _context.Equipment.FindAsync(equipmentId);
            if (equipment == null)
                return ServiceResult.Fail("Equipment not found.");

            // Authorization: Only owner can delete
            if (equipment.OwnerId != requestingUserId)
                return ServiceResult.Fail("You are not authorized to delete this equipment.");

            // Business Logic: Check if equipment has active loans
            var hasActiveLoans = await _context.Loans.AnyAsync(l =>
                l.EquipmentId == equipmentId &&
                (l.Status == "APPROVED" || l.Status == "PENDING" || l.Status == "EXTENSION_PENDING"));

            if (hasActiveLoans)
                return ServiceResult.Fail("Cannot delete equipment with active or pending loans.");

            _context.Equipment.Remove(equipment);
            await _context.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        #endregion

        #region --- Authorization Helpers ---

        public async Task<bool> IsOwner(long equipmentId, long userId)
        {
            return await _context.Equipment.AnyAsync(e => e.Id == equipmentId && e.OwnerId == userId);
        }

        #endregion

        #region --- Mapping ---

        private static EquipmentResponseDto MapToResponseDto(Equipment equipment) => new()
        {
            Id = equipment.Id,
            Name = equipment.Name,
            Description = equipment.Description,
            Category = equipment.Category,
            OwnerId = equipment.OwnerId,
            CreatedAt = equipment.CreatedAt
        };

        #endregion
    }
}