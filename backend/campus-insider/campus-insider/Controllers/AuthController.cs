using campus_insider.DTOs;
using campus_insider.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace campus_insider.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserService _userService;

        public AuthController(IConfiguration config, UserService userService)
        {
            _config = config;
            _userService = userService;
        }

        // POST /api/auth/login
        [EnableRateLimiting("login-policy")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            // Validation 1: School email domain
            if (!login.Email.EndsWith("@lycee-rene-cassin.fr"))
                return BadRequest(new { message = "Only school emails (@lycee-rene-cassin.fr) are permitted." });

            // Validation 2: Authenticate user
            var result = await _userService.ValidateLoginAsync(login);
            if (!result.Success)
                return Unauthorized(new { message = result.ErrorMessage });

            var user = result.Data!;

            // Create JWT claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            };

            // Generate token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new AuthResponseDto
            {
                Token = tokenString,
                ExpiresAt = token.ValidTo,
                User = new UserResponseDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
                }
            });
        }

        // POST /api/auth/refresh (Optional but recommended)
        [HttpPost("refresh")]
        public IActionResult RefreshToken()
        {
            // TODO: Implement refresh token logic if needed
            // This would use a separate refresh token stored in DB
            return Ok(new { message = "Refresh token endpoint - to be implemented" });
        }


        //[HttpGet("me")]
        //public async Task<IActionResult> GetCurrentUser()
        //{
        //    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        //    string? token = await HttpContext.GetTokenAsync("access_token");

        //    if (userIdClaim == null)
        //        return Unauthorized();

        //    if (!long.TryParse(userIdClaim.Value, out var userId))
        //        return Unauthorized();

        //    var user = await _userService.GetByIdAsync(userId);
        //    if (user == null)
        //        return NotFound();
        //    return Ok(new UserResponseDto
        //    {
        //        Id = user.Id,
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        Email = user.Email,
        //        Role = user.Role,
        //        CreatedAt = user.CreatedAt
        //    });
        //}

        #region --- DTOs ---

        public class AuthResponseDto
        {
            public string Token { get; set; } = string.Empty;
            public DateTime ExpiresAt { get; set; }
            public UserResponseDto User { get; set; } = null!;
        }

        #endregion
    }
}