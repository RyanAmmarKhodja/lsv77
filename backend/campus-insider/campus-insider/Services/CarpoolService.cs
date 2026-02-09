using campus_insider.Data;
using campus_insider.DTOs;
using campus_insider.Models;
using Microsoft.EntityFrameworkCore;

namespace campus_insider.Services
{
    public class CarpoolService
    {
        private readonly AppDbContext _context;

        public CarpoolService(AppDbContext context)
        {
            _context = context;
        }

        #region --- Queries ---

        public async Task<PagedResult<CarpoolResponseDto>> GetAllCarpools(
            int pageNumber = 1,
            int pageSize = 20)
        {
            var query = _context.CarpoolTrips
                .AsNoTracking()
                .Include(c => c.Driver)
                .Include(c => c.Passengers)
                .ThenInclude(p => p.User);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(c => c.DepartureTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CarpoolResponseDto>
            {
                Items = items.Select(MapToResponseDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<CarpoolResponseDto>> SearchCarpools(
            string? departure = null,
            string? destination = null,
            DateTime? departureDate = null,
            int pageNumber = 1,
            int pageSize = 20)
        {
            var query = _context.CarpoolTrips
                .AsNoTracking()
                .Include(c => c.Driver)
                .Include(c => c.Passengers)
                .ThenInclude(p => p.User)
                .Where(c => c.Status == "PENDING" && c.AvailableSeats > 0);

            if (!string.IsNullOrWhiteSpace(departure))
            {
                departure = departure.ToLower();
                query = query.Where(c => c.Departure.ToLower().Contains(departure));
            }

            if (!string.IsNullOrWhiteSpace(destination))
            {
                destination = destination.ToLower();
                query = query.Where(c => c.Destination.ToLower().Contains(destination));
            }

            if (departureDate.HasValue)
            {
                var startOfDay = departureDate.Value.Date;
                var endOfDay = startOfDay.AddDays(1);
                query = query.Where(c => c.DepartureTime >= startOfDay && c.DepartureTime < endOfDay);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.DepartureTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CarpoolResponseDto>
            {
                Items = items.Select(MapToResponseDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<CarpoolResponseDto>> GetCarpoolsByUser(
            long userId,
            string? status = null,
            int pageNumber = 1,
            int pageSize = 20)
        {
            var query = _context.CarpoolTrips
                .AsNoTracking()
                .Include(c => c.Driver)
                .Include(c => c.Passengers)
                .ThenInclude(p => p.User)
                .Where(c => c.DriverId == userId || c.Passengers.Any(p => p.UserId == userId));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(c => c.Status == status);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(c => c.DepartureTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CarpoolResponseDto>
            {
                Items = items.Select(MapToResponseDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<CarpoolResponseDto>> GetCarpoolsByDriver(
            long driverId,
            int pageNumber = 1,
            int pageSize = 20)
        {
            var query = _context.CarpoolTrips
                .AsNoTracking()
                .Include(c => c.Driver)
                .Include(c => c.Passengers)
                .ThenInclude(p => p.User)
                .Where(c => c.DriverId == driverId);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(c => c.DepartureTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CarpoolResponseDto>
            {
                Items = items.Select(MapToResponseDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<CarpoolResponseDto?> GetCarpoolByIdAsync(long id)
        {
            var carpool = await _context.CarpoolTrips
                .AsNoTracking()
                .Include(c => c.Driver)
                .Include(c => c.Passengers)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            return carpool != null ? MapToResponseDto(carpool) : null;
        }

        #endregion

        #region --- Commands ---

        public async Task<ServiceResult<CarpoolResponseDto>> CreateRide(CarpoolCreateDto dto, long driverId)
        {
            // Validation 1: Available seats
            if (dto.AvailableSeats <= 0 || dto.AvailableSeats > 8)
                return ServiceResult<CarpoolResponseDto>.Fail("Available seats must be between 1 and 8.");

            // Validation 2: Departure location
            if (string.IsNullOrWhiteSpace(dto.Departure))
                return ServiceResult<CarpoolResponseDto>.Fail("Departure location is required.");

            // Validation 3: Destination
            if (string.IsNullOrWhiteSpace(dto.Destination))
                return ServiceResult<CarpoolResponseDto>.Fail("Destination is required.");

            // Validation 4: Departure time must be in future
            if (dto.DepartureTime <= DateTime.UtcNow)
                return ServiceResult<CarpoolResponseDto>.Fail("Departure time must be in the future.");

            // Validation 5: Departure time not too far in future (e.g., max 3 months)
            if (dto.DepartureTime > DateTime.UtcNow.AddMonths(3))
                return ServiceResult<CarpoolResponseDto>.Fail("Departure time cannot be more than 3 months in advance.");

            var carpool = new CarpoolTrip
            {
                DriverId = driverId,
                Departure = dto.Departure.Trim(),
                Destination = dto.Destination.Trim(),
                DepartureTime = dto.DepartureTime,
                Status = "PENDING",
                AvailableSeats = dto.AvailableSeats,
                CreatedAt = DateTime.UtcNow
            };

            _context.CarpoolTrips.Add(carpool);
            await _context.SaveChangesAsync();

            // Reload with includes
            var created = await _context.CarpoolTrips
                .Include(c => c.Driver)
                .Include(c => c.Passengers)
                .FirstOrDefaultAsync(c => c.Id == carpool.Id);

            return created != null
                ? ServiceResult<CarpoolResponseDto>.Ok(MapToResponseDto(created))
                : ServiceResult<CarpoolResponseDto>.Fail("Failed to create carpool.");
        }

        public async Task<ServiceResult> CancelRide(long carpoolId, long userId)
        {
            var carpool = await _context.CarpoolTrips
                .Include(c => c.Passengers)
                .FirstOrDefaultAsync(c => c.Id == carpoolId);

            if (carpool == null)
                return ServiceResult.Fail("Carpool not found.");

            // Authorization: Only driver can cancel
            if (carpool.DriverId != userId)
                return ServiceResult.Fail("Only the driver can cancel the ride.");

            // Business Logic: Can't cancel if already started or completed
            if (carpool.Status == "IN_PROGRESS")
                return ServiceResult.Fail("Cannot cancel a ride that is already in progress.");

            if (carpool.Status == "COMPLETED")
                return ServiceResult.Fail("Cannot cancel a completed ride.");

            if (carpool.Status == "CANCELLED")
                return ServiceResult.Fail("Ride is already cancelled.");

            // Business Logic: Warn if passengers exist (but still allow cancellation)
            // In production, you'd want to notify passengers here

            carpool.Status = "CANCELLED";
            await _context.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> StartRide(long carpoolId, long userId)
        {
            var carpool = await _context.CarpoolTrips.FindAsync(carpoolId);
            if (carpool == null)
                return ServiceResult.Fail("Carpool not found.");

            // Authorization: Only driver can start
            if (carpool.DriverId != userId)
                return ServiceResult.Fail("Only the driver can start the ride.");

            // Business Logic: Must be in PENDING status
            if (carpool.Status != "PENDING")
                return ServiceResult.Fail("Only pending rides can be started.");

            // Business Logic: Can't start too early (e.g., more than 1 hour before scheduled time)
            if (DateTime.UtcNow < carpool.DepartureTime.AddHours(-1))
                return ServiceResult.Fail("Cannot start ride more than 1 hour before scheduled departure.");

            carpool.Status = "IN_PROGRESS";
            await _context.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> CompleteRide(long carpoolId, long userId)
        {
            var carpool = await _context.CarpoolTrips.FindAsync(carpoolId);
            if (carpool == null)
                return ServiceResult.Fail("Carpool not found.");

            // Authorization: Only driver can complete
            if (carpool.DriverId != userId)
                return ServiceResult.Fail("Only the driver can complete the ride.");

            // Business Logic: Must be in progress
            if (carpool.Status != "IN_PROGRESS")
                return ServiceResult.Fail("Only rides in progress can be completed.");

            carpool.Status = "COMPLETED";
            await _context.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> JoinRide(long carpoolId, long userId)
        {
            var carpool = await _context.CarpoolTrips
                .Include(c => c.Passengers)
                .FirstOrDefaultAsync(c => c.Id == carpoolId);

            if (carpool == null)
                return ServiceResult.Fail("Carpool not found.");

            // Business Logic: Can't join own ride
            if (carpool.DriverId == userId)
                return ServiceResult.Fail("You cannot join your own ride.");

            // Business Logic: Must be PENDING
            if (carpool.Status != "PENDING")
                return ServiceResult.Fail("Can only join pending rides.");

            // Business Logic: Already a passenger?
            if (carpool.Passengers.Any(p => p.UserId == userId))
                return ServiceResult.Fail("You have already joined this ride.");

            // Business Logic: Check available seats
            if (carpool.AvailableSeats <= 0)
                return ServiceResult.Fail("No available seats.");

            // Business Logic: Can't join if departure is too soon (e.g., less than 15 minutes)
            if (carpool.DepartureTime <= DateTime.UtcNow.AddMinutes(15))
                return ServiceResult.Fail("Cannot join a ride departing in less than 15 minutes.");

            // Add passenger and decrement available seats
            _context.CarpoolPassengers.Add(new CarpoolPassenger
            {
                CarpoolTripId = carpoolId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            });

            carpool.AvailableSeats--;
            await _context.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> LeaveRide(long carpoolId, long userId)
        {
            var carpool = await _context.CarpoolTrips
                .Include(c => c.Passengers)
                .FirstOrDefaultAsync(c => c.Id == carpoolId);

            if (carpool == null)
                return ServiceResult.Fail("Carpool not found.");

            // Business Logic: Find passenger record
            var passenger = carpool.Passengers.FirstOrDefault(p => p.UserId == userId);
            if (passenger == null)
                return ServiceResult.Fail("You are not a passenger on this ride.");

            // Business Logic: Can't leave if ride already started
            if (carpool.Status == "IN_PROGRESS")
                return ServiceResult.Fail("Cannot leave a ride that has already started.");

            if (carpool.Status == "COMPLETED")
                return ServiceResult.Fail("Cannot leave a completed ride.");

            // Business Logic: Can't leave if departure is too soon (e.g., less than 1 hour)
            if (carpool.DepartureTime <= DateTime.UtcNow.AddHours(1))
                return ServiceResult.Fail("Cannot leave a ride departing in less than 1 hour.");

            // Remove passenger and increment available seats
            _context.CarpoolPassengers.Remove(passenger);
            carpool.AvailableSeats++;
            await _context.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteRide(long carpoolId, long userId)
        {
            var carpool = await _context.CarpoolTrips
                .Include(c => c.Passengers)
                .FirstOrDefaultAsync(c => c.Id == carpoolId);

            if (carpool == null)
                return ServiceResult.Fail("Carpool not found.");

            // Authorization: Only driver can delete
            if (carpool.DriverId != userId)
                return ServiceResult.Fail("Only the driver can delete the ride.");

            // Business Logic: Can't delete if has passengers
            if (carpool.Passengers.Any())
                return ServiceResult.Fail("Cannot delete a ride with passengers. Cancel it instead.");

            // Business Logic: Can't delete if already started or completed
            if (carpool.Status == "IN_PROGRESS" || carpool.Status == "COMPLETED")
                return ServiceResult.Fail("Cannot delete rides that are in progress or completed.");

            _context.CarpoolTrips.Remove(carpool);
            await _context.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        #endregion

        #region --- Authorization Helpers ---

        public async Task<bool> IsDriver(long carpoolId, long userId)
        {
            return await _context.CarpoolTrips.AnyAsync(c => c.Id == carpoolId && c.DriverId == userId);
        }

        public async Task<bool> IsPassenger(long carpoolId, long userId)
        {
            return await _context.CarpoolPassengers.AnyAsync(p => p.CarpoolTripId == carpoolId && p.UserId == userId);
        }

        #endregion

        #region --- Mapping ---

        private CarpoolResponseDto MapToResponseDto(CarpoolTrip carpool)
        {
            return new CarpoolResponseDto
            {
                Id = carpool.Id,
                Departure = carpool.Departure,
                Destination = carpool.Destination,
                DepartureTime = carpool.DepartureTime,
                Status = carpool.Status,
                AvailableSeats = carpool.AvailableSeats,
                TotalSeats = carpool.AvailableSeats + (carpool.Passengers?.Count ?? 0),
                Driver = carpool.Driver != null ? new UserResponseDto
                {
                    Id = carpool.Driver.Id,
                    FirstName = carpool.Driver.FirstName,
                    LastName = carpool.Driver.LastName,
                    Email = carpool.Driver.Email,
                    Role = carpool.Driver.Role,
                    CreatedAt = carpool.Driver.CreatedAt
                } : null,
                Passengers = carpool.Passengers?.Select(p => new UserResponseDto
                {
                    Id = p.User.Id,
                    FirstName = p.User.FirstName,
                    LastName = p.User.LastName,
                    Email = p.User.Email,
                    Role = p.User.Role,
                    CreatedAt = p.User.CreatedAt
                }).ToList() ?? new List<UserResponseDto>(),
                CreatedAt = carpool.CreatedAt
            };
        }

        #endregion
    }
}