using campus_insider.Data;
using campus_insider.DTOs;
using campus_insider.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace campus_insider.Services
{
    public class LoanService
    {
        private readonly AppDbContext _context;

        public LoanService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<LoanDto>> GetAllLoans()
        {
            return await _context.Loans
                .AsNoTracking()
                .Select(l => MapToDto(l))
                .ToListAsync();
        }

        public async Task<LoanDto> GetLoanById(long id)
        {
            var loan = await _context.Loans.FindAsync(id);
            return loan == null ? null : MapToDto(loan);
        }

        public async Task<bool> DeleteLoan(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return false;

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
            return true;
        }

        // --- Specific Methods ---
        // ALL USERS ACTIONS
        public async Task RequestLoan(LoanDto request)
        {
            var loan = new Loan
            {
                EquipmentId = request.EquipmentId,
                BorrowerId = request.BorrowerId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow
            };

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
        }

        public async Task CancelLoan(long loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null) return;

            // Logic: Only allow cancellation if the loan hasn't started yet
            if (loan.Status == "PENDING" || (loan.Status == "APPROVED" && loan.StartDate > DateTime.UtcNow))
            {
                loan.Status = "CANCELLED";
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExtendLoan(int loanId, DateTime newEndDate)
        {
            var loan = await _context.Loans.Include(l => l.Equipment).FirstOrDefaultAsync(l => l.Id == loanId);
            if (loan == null || loan.Status != "APPROVED") return false;

            // Check if equipment is reserved for the new requested period
            bool isReserved = await _context.Loans.AnyAsync(l =>
                l.EquipmentId == loan.EquipmentId &&
                l.Id != loanId &&
                l.Status == "APPROVED" &&
                newEndDate > l.StartDate &&
                loan.EndDate < l.EndDate);

            if (isReserved) return false;

            // Logic: Set to EXTENSION_PENDING; owner must approve before EndDate is officially updated
            loan.Status = "EXTENSION_PENDING";
            // Store requested date in a temporary field or metadata if available
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task CompleteLoan(long loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null) return;

            // Logic: Set status to COMPLETED and log actual return time
            loan.Status = "COMPLETED";
            loan.EndDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<List<LoanDto>> GetOngoingLoans()
        {
            var now = DateTime.UtcNow;
            return await _context.Loans
                .Where(l => l.Status == "APPROVED" && l.StartDate <= now && l.EndDate >= now)
                .AsNoTracking()
                .Select(l => MapToDto(l))
                .ToListAsync();
        }

        public async Task<List<LoanDto>> GetLoansByEquipment(long equipmentId)
        {
            return await _context.Loans
                .Where(l => l.EquipmentId == equipmentId)
                .AsNoTracking()
                .Select(l => MapToDto(l))
                .ToListAsync();
        }

        public async Task<List<LoanDto>> GetLoansByStatus(string status)
        {
            return await _context.Loans
                .Where(l => l.Status == status)
                .AsNoTracking()
                .Select(l => MapToDto(l))
                .ToListAsync();
        }


        // USER ACTIONS
        public async Task<List<LoanDto>> GetAllUserLoans(long userId)
        {
            return await _context.Loans
                .Where(l => l.BorrowerId == userId)
                .AsNoTracking()
                .Select(l => MapToDto(l))
                .ToListAsync();
        }

        public async Task<List<LoanDto>> GetUserLoansByStatus(long userId, string status)
        {
            return await _context.Loans
                .Where(l => l.BorrowerId == userId && l.Status == status)
                .AsNoTracking()
                .Select(l => MapToDto(l))
                .ToListAsync();
        }

        public async Task<List<LoanDto>> GetUserOverdueLoans(long userId)
        {
            var now = DateTime.UtcNow;
            return await _context.Loans
                .Where(l => l.BorrowerId == userId && l.Status == "APPROVED" && l.EndDate < now)
                .AsNoTracking()
                .Select(l => MapToDto(l))
                .ToListAsync();
        }


        // OWNER ACTIONS
        public async Task<List<LoanDto>> GetOwnerLoansByStatus(long userId, string status)
        {
            return await _context.Loans
                .Include(l => l.Equipment)
                .Where(l => l.Equipment.OwnerId == userId && l.Status == status)
                .AsNoTracking()
                .Select(l => MapToDto(l))
                .ToListAsync();
        }

        public async Task<List<LoanDto>> GetOwnerOngoingLoans(long userId)
        {
            var now = DateTime.UtcNow;
            return await _context.Loans
                .Include(l => l.Equipment)
                .Where(l => l.Equipment.OwnerId == userId && l.Status == "APPROVED" && l.StartDate <= now && l.EndDate >= now)
                .AsNoTracking()
                .Select(l => MapToDto(l))
                .ToListAsync();
        }

        public async Task<List<LoanDto>> GetOwnerOverdueLoans(long userId)
        {
            var now = DateTime.UtcNow;
            return await _context.Loans
                .Include(l => l.Equipment)
                .Where(l => l.Equipment.OwnerId == userId && l.Status == "APPROVED" && l.EndDate < now)
                .AsNoTracking()
                .Select(l => MapToDto(l))
                .ToListAsync();
        }

        public async Task RejectLoan(long loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null) return;

            // Decision: Keep in DB as DENIED for audit trail/history purposes
            loan.Status = "DENIED";
            await _context.SaveChangesAsync();
        }

        public async Task ApproveLoan(long loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null) return;

            loan.Status = "APPROVED";

            // Logical addition: Mark the equipment as unavailable or handle concurrency here
            await _context.SaveChangesAsync();
        }

        public async Task ApproveExtension(long loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null || loan.Status != "EXTENSION_PENDING") return;
            // Logic: Update the EndDate to the requested extension date and set status back to APPROVED
            // Assuming we stored the requested extension date in a temporary field or metadata
            // loan.EndDate = loan.RequestedExtensionDate;
            loan.Status = "APPROVED";
            await _context.SaveChangesAsync();
        }

        public async Task RejectExtension(long loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null || loan.Status != "EXTENSION_PENDING") return;
            // Logic: Revert status back to APPROVED without changing EndDate
            loan.Status = "APPROVED";
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsEquipmentOwner(long loanId, long userId)
        {
            var loan = await _context.Loans
                .Include(l => l.Equipment)
                .FirstOrDefaultAsync(l => l.Id == loanId);
            return loan != null && loan.Equipment.OwnerId == userId;
        }

        private static LoanDto MapToDto(Loan loan)
        {
            return new LoanDto
            {
                Id = loan.Id,
                StartDate = loan.StartDate,
                EndDate = loan.EndDate,
                Status = loan.Status,
                CreatedAt = loan.CreatedAt
                // Borrower and Equipment mapping handled via AutoMapper or manual assignment
            };
        }
    }
}
