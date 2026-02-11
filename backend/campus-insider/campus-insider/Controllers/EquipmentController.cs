using campus_insider.DTOs;
using campus_insider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace campus_insider.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/equipment")]
    public class EquipmentController : ControllerBase
    {
        private readonly EquipmentService _equipmentService;

        public EquipmentController(EquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        private long GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(userIdString, out long userId) ? userId : 0;
        }

        #region --- Public Queries ---

        // GET /api/equipment?pageNumber=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<PagedResult<EquipmentResponseDto>>> GetEquipment(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _equipmentService.GetAllEquipment(pageNumber, pageSize);
            return Ok(result);
        }

        // GET /api/equipment/search?searchTerm=laptop&category=electronics&pageNumber=1
        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<EquipmentResponseDto>>> SearchEquipment(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? category = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _equipmentService.SearchEquipment(searchTerm, category, pageNumber, pageSize);
            return Ok(result);
        }

        // GET /api/equipment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EquipmentResponseDto>> GetById(long id)
        {
            var equipment = await _equipmentService.GetByEquipmentIdAsync(id);
            if (equipment == null)
                return NotFound(new { message = "Equipment not found." });

            return Ok(equipment);
        }

        // GET /api/equipment/my-equipment?pageNumber=1&pageSize=20
        [HttpGet("my-equipment")]
        public async Task<ActionResult<PagedResult<EquipmentResponseDto>>> GetMyEquipment(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _equipmentService.GetEquipmentByOwner(userId, pageNumber, pageSize);
            return Ok(result);
        }

        #endregion

        #region --- Owner Actions ---

        // POST /api/equipment
        [HttpPost]
        public async Task<ActionResult<EquipmentResponseDto>> Create([FromBody] EquipmentCreateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _equipmentService.ShareEquipment(dto, userId);
            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Data!.Id },
                result.Data);
        }

        // PUT /api/equipment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] EquipmentUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _equipmentService.UpdateEquipment(id, dto, userId);
            if (!result.Success)
            {
                // Differentiate between authorization and validation errors
                if (result.ErrorMessage!.Contains("not authorized"))
                    return Forbid();

                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { message = "Equipment updated successfully." });
        }

        // DELETE /api/equipment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _equipmentService.UnshareEquipment(id, userId);
            if (!result.Success)
            {
                if (result.ErrorMessage!.Contains("not authorized"))
                    return Forbid();

                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { message = "Equipment deleted successfully." });
        }

        #endregion
    }
}