using campus_insider.DTOs;
using campus_insider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace campus_insider.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        private long GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(userIdString, out long userId) ? userId : 0;
        }

        #region --- Public Endpoints ---

        // POST /api/users/register
        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register([FromBody] UserCreateDto dto)
        {
            var result = await _userService.RegisterAsync(dto);

            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetUserById), new { id = result.Data!.Id }, result.Data);
        }

        #endregion

        #region --- Authorized Endpoints ---

        // GET /api/users/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUserById(long id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }

        // GET /api/users
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<UserResponseDto>>> GetUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // GET /api/users/me
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserResponseDto>> GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }

        // PUT /api/users/me
        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _userService.UpdateAsync(userId, dto);

            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { message = "Profile updated successfully." });
        }

        // GET /api/users/search?q=term
        [Authorize]
        [HttpGet("search")]
        public async Task<ActionResult<List<UserResponseDto>>> SearchUsers([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                return Ok(new List<UserResponseDto>());

            var users = await _userService.SearchUsersAsync(q);
            return Ok(users);
        }

        #endregion
    }
}