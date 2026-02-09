using campus_insider.DTOs;
using campus_insider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace campus_insider.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/carpools")]
    public class CarpoolController : ControllerBase
    {
        private readonly CarpoolService _carpoolService;

        public CarpoolController(CarpoolService carpoolService)
        {
            _carpoolService = carpoolService;
        }

        private long GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(userIdString, out long userId) ? userId : 0;
        }

        #region --- Public Queries ---

        // GET /api/carpools?pageNumber=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<PagedResult<CarpoolResponseDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _carpoolService.GetAllCarpools(pageNumber, pageSize);
            return Ok(result);
        }

        // GET /api/carpools/search?departure=Paris&destination=Lyon&departureDate=2026-02-10
        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<CarpoolResponseDto>>> Search(
            [FromQuery] string? departure = null,
            [FromQuery] string? destination = null,
            [FromQuery] DateTime? departureDate = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _carpoolService.SearchCarpools(departure, destination, departureDate, pageNumber, pageSize);
            return Ok(result);
        }

        // GET /api/carpools/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CarpoolResponseDto>> GetById(long id)
        {
            var carpool = await _carpoolService.GetCarpoolByIdAsync(id);
            if (carpool == null)
                return NotFound(new { message = "Carpool not found." });

            return Ok(carpool);
        }

        // GET /api/carpools/my-rides?status=PENDING&pageNumber=1&pageSize=20
        [HttpGet("my-rides")]
        public async Task<ActionResult<PagedResult<CarpoolResponseDto>>> GetMyRides(
            [FromQuery] string? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _carpoolService.GetCarpoolsByUser(userId, status, pageNumber, pageSize);
            return Ok(result);
        }

        // GET /api/carpools/my-drives?pageNumber=1&pageSize=20
        [HttpGet("my-drives")]
        public async Task<ActionResult<PagedResult<CarpoolResponseDto>>> GetMyDrives(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _carpoolService.GetCarpoolsByDriver(userId, pageNumber, pageSize);
            return Ok(result);
        }

        #endregion

        #region --- Driver Actions ---

        // POST /api/carpools
        [HttpPost]
        public async Task<ActionResult<CarpoolResponseDto>> CreateRide([FromBody] CarpoolCreateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _carpoolService.CreateRide(dto, userId);
            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Data!.Id },
                result.Data);
        }

        // PATCH /api/carpools/5/cancel
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelRide(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _carpoolService.CancelRide(id, userId);
            if (!result.Success)
            {
                if (result.ErrorMessage!.Contains("Only the driver"))
                    return Forbid();

                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { message = "Ride cancelled successfully." });
        }

        // PATCH /api/carpools/5/start
        [HttpPatch("{id}/start")]
        public async Task<IActionResult> StartRide(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _carpoolService.StartRide(id, userId);
            if (!result.Success)
            {
                if (result.ErrorMessage!.Contains("Only the driver"))
                    return Forbid();

                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { message = "Ride started successfully." });
        }

        // PATCH /api/carpools/5/complete
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> CompleteRide(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _carpoolService.CompleteRide(id, userId);
            if (!result.Success)
            {
                if (result.ErrorMessage!.Contains("Only the driver"))
                    return Forbid();

                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { message = "Ride completed successfully." });
        }

        // DELETE /api/carpools/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRide(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _carpoolService.DeleteRide(id, userId);
            if (!result.Success)
            {
                if (result.ErrorMessage!.Contains("Only the driver"))
                    return Forbid();

                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { message = "Ride deleted successfully." });
        }

        #endregion

        #region --- Passenger Actions ---

        // POST /api/carpools/5/join
        [HttpPost("{id}/join")]
        public async Task<IActionResult> JoinRide(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _carpoolService.JoinRide(id, userId);
            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { message = "Successfully joined the ride." });
        }

        // POST /api/carpools/5/leave
        [HttpPost("{id}/leave")]
        public async Task<IActionResult> LeaveRide(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var result = await _carpoolService.LeaveRide(id, userId);
            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { message = "Successfully left the ride." });
        }

        #endregion
    }
}