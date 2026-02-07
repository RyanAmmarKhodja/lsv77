using campus_insider.DTOs;
using campus_insider.Models;
using campus_insider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace campus_insider.Controllers
{

    [ApiController]
    [Authorize]
    [Route("api/loans")]
    public class LoansController : ControllerBase
    {
        private readonly LoanService _loanService;

        public LoansController(LoanService loanService)
        {
            _loanService = loanService;
        }

        private long GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(userIdString, out long userId) ? userId : 0;
        }

        // BASIC CRUD

        [HttpGet]
        public async Task<ActionResult<List<LoanDto>>> GetAll()
        {
            return Ok(await _loanService.GetAllLoans());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LoanDto>> GetById(int id)
        {
            var loan = await _loanService.GetLoanById(id);
            if (loan == null) return NotFound();
            return Ok(loan);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _loanService.DeleteLoan(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // GENERAL ACTIONS

        [HttpGet("pending")]
        public async Task<ActionResult<List<LoanDto>>> GetPending(string status)
        {
            return Ok(await _loanService.GetLoansByStatus(status));
        }

        [HttpGet("ongoing")]
        public async Task<ActionResult<List<LoanDto>>> GetOngoing()
        {
            return Ok(await _loanService.GetOngoingLoans());
        }




        // ALL USERS ACTIONS
        [HttpPost("request")]
        public async Task<IActionResult> RequestLoan([FromBody] LoanDto request)
        {
            var userId = GetCurrentUserId();

            var loan = new LoanDto
            {
                BorrowerId = userId,
                EquipmentId = request.EquipmentId,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };
            await _loanService.RequestLoan(loan);
            return StatusCode(201);
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> Cancel(long id)
        {
            var userId = GetCurrentUserId();
            var loan = await _loanService.GetLoanById(id);
            if (loan.BorrowerId!=userId) return Forbid();
            await _loanService.CancelLoan(id);
            return Ok();
        }

        [HttpPatch("{id}/extend")]
        public async Task<IActionResult> Extend(int id, [FromBody] DateTime newEndDate)
        {
            var success = await _loanService.ExtendLoan(id, newEndDate);
            if (!success) return BadRequest("Extension unavailable or loan not eligible.");
            return Ok();
        }

        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> Complete(long id)
        {
            var userId = GetCurrentUserId(); 
            var loan = await _loanService.GetLoanById(id);
            if (loan.BorrowerId != userId) return Forbid();

            await _loanService.CompleteLoan(id);
            return Ok();
        }
        
        [HttpGet("/user/loans")]
        public async Task<ActionResult<List<LoanDto>>> GetAllUserLoans()
        {
            var userId = GetCurrentUserId();
            return Ok(await _loanService.GetAllUserLoans(userId));
        }

        // USER ACTIONS
        [HttpGet("/equipment/{equipmentId}")]
        public async Task<ActionResult<List<LoanDto>>> GetLoansByEquipment(long equipmentId)
        {
            var userId = GetCurrentUserId();
            return Ok(await _loanService.GetLoansByEquipment(equipmentId));
        }

        [HttpGet("/user/loans/{status}")]
        public async Task<ActionResult<List<LoanDto>>> GetUserLoansByStatus(string status)
        {
            var userId = GetCurrentUserId();
            return Ok(await _loanService.GetUserLoansByStatus(userId, status));
        }

        [HttpGet("/user/loans/overdue")]
        public async Task<ActionResult<List<LoanDto>>> GetUserOverdueLoans()
        {
            var userId = GetCurrentUserId();
            return Ok(await _loanService.GetUserOverdueLoans(userId));
        }


        // OWNER ACTIONS
        [HttpPatch("{id}/approve")]
        public async Task<IActionResult> Approve(long id)
        {
            var UserIdString = long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out long userId);
            var isOwner = await _loanService.IsEquipmentOwner(id, userId);

            if (!isOwner) return Forbid();

            await _loanService.ApproveLoan(id);
            return Ok();
        }

        [HttpPatch("{id}/reject")]
        public async Task<IActionResult> Reject(long id)
        {
            var UserIdString = long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out long userId);
            var isOwner = await _loanService.IsEquipmentOwner(id, userId);
            if (!isOwner) return Forbid();

            await _loanService.RejectLoan(id);
            return Ok();
        }

        [HttpGet("/owner/overdue")]
        public async Task<ActionResult<List<LoanDto>>> GetOverdueLoans()
        {
            var userId = GetCurrentUserId();
            return Ok(await _loanService.GetOwnerOverdueLoans(userId));

        }

        [HttpGet("/owner/ongoing")]
        public async Task<ActionResult<List<LoanDto>>> GetOngoingLoans()
        {
            var userId = GetCurrentUserId();
            return Ok(await _loanService.GetOwnerOngoingLoans(userId));

        }

        [HttpGet("/owner/{status}")]
        public async Task<ActionResult<List<LoanDto>>> GetOngoingLoans(string status)
        {
            var userId = GetCurrentUserId();
            return Ok(await _loanService.GetOwnerLoansByStatus(userId, status));

        }
    }
}