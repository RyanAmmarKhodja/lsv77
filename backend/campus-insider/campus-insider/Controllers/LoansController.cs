using campus_insider.DTOs;
using campus_insider.Services;
using Microsoft.AspNetCore.Mvc;

namespace campus_insider.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly LoanService _loanService;

        public LoansController(LoanService loanService)
        {
            _loanService = loanService;
        }

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

        [HttpPost("request")]
        public async Task<IActionResult> RequestLoan([FromBody] LoanDto request)
        {
            await _loanService.RequestLoan(request);
            return StatusCode(201);
        }

        [HttpPatch("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            await _loanService.ApproveLoan(id);
            return Ok();
        }

        [HttpPatch("{id}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            await _loanService.RejectLoan(id);
            return Ok();
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
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
        public async Task<IActionResult> Complete(int id)
        {
            await _loanService.CompleteLoan(id);
            return Ok();
        }

        [HttpGet("pending")]
        public async Task<ActionResult<List<LoanDto>>> GetPending()
        {
            return Ok(await _loanService.GetPendingLoans());
        }

        [HttpGet("ongoing")]
        public async Task<ActionResult<List<LoanDto>>> GetOngoing()
        {
            return Ok(await _loanService.GetOngoingLoans());
        }
    }
}
