using campus_insider.Data;
using campus_insider.DTOs;
using campus_insider.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace campus_insider.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        #region --- Queries ---

        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();
            return users.Select(MapToResponseDto).ToList();
        }

        public async Task<UserResponseDto?> GetByIdAsync(long id)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            return user != null ? MapToResponseDto(user) : null;
        }

        public async Task<List<UserResponseDto>> SearchUsersAsync(string query)
        {
            var q = query.ToLower().Trim();
            var users = await _context.Users
                .AsNoTracking()
                .Where(u => u.FirstName.ToLower().Contains(q) ||
                            u.LastName.ToLower().Contains(q) ||
                            u.Email.ToLower().Contains(q))
                .Take(20)
                .ToListAsync();
            return users.Select(MapToResponseDto).ToList();
        }

        #endregion

        #region --- Authentication ---

        public async Task<ServiceResult<UserResponseDto>> RegisterAsync(UserCreateDto dto)
        {
            // Validation 1: Email domain check
            if (!dto.Email.EndsWith("@lsv77.fr"))
                return ServiceResult<UserResponseDto>.Fail("Only school emails (@lsv77.fr) are permitted.");

            // Validation 2: Email already exists
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return ServiceResult<UserResponseDto>.Fail("Email already registered.");

            // Validation 3: Password strength
            if (dto.Password.Length < 8)
                return ServiceResult<UserResponseDto>.Fail("Password must be at least 8 characters long.");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email.ToLower().Trim(), // Normalize email
                Password = HashPassword(dto.Password),
                Role = "USER", // Default role
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return ServiceResult<UserResponseDto>.Ok(MapToResponseDto(user));
        }

        public async Task<ServiceResult<User>> ValidateLoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower().Trim());

            if (user == null)
                return ServiceResult<User>.Fail("Invalid email or password.");

            if (!VerifyPassword(dto.Password, user.Password))
                return ServiceResult<User>.Fail("Invalid email or password.");

            return ServiceResult<User>.Ok(user);
        }

        #endregion

        #region --- Updates ---

        public async Task<ServiceResult> UpdateAsync(long userId, UserUpdateDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return ServiceResult.Fail("User not found.");

            // Email change validation
            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
            {
                if (!dto.Email.EndsWith("@lycee-rene-cassin.fr"))
                    return ServiceResult.Fail("Only school emails are permitted.");

                if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != userId))
                    return ServiceResult.Fail("Email already in use.");

                user.Email = dto.Email.ToLower().Trim();
            }

            // Update fields if provided
            if (!string.IsNullOrEmpty(dto.FirstName))
                user.FirstName = dto.FirstName;

            if (!string.IsNullOrEmpty(dto.LastName))
                user.LastName = dto.LastName;

            // Password change
            if (!string.IsNullOrEmpty(dto.Password))
            {
                if (dto.Password.Length < 8)
                    return ServiceResult.Fail("Password must be at least 8 characters long.");

                user.Password = HashPassword(dto.Password);
            }

            await _context.SaveChangesAsync();
            return ServiceResult.Ok();
        }

        #endregion

        #region --- Password Hashing ---

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var salt = GenerateSalt();
            var saltedPassword = password + salt;
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));

            // Format: salt:hash
            return $"{salt}:{Convert.ToBase64String(hashBytes)}";
        }

        private static bool VerifyPassword(string inputPassword, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            var salt = parts[0];
            var hash = parts[1];

            using var sha256 = SHA256.Create();
            var saltedInput = inputPassword + salt;
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedInput));
            var inputHash = Convert.ToBase64String(hashBytes);

            return hash == inputHash;
        }

        private static string GenerateSalt()
        {
            var saltBytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        #endregion

        #region --- Mapping ---

        private static UserResponseDto MapToResponseDto(User user) => new UserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };

        #endregion
    }
}