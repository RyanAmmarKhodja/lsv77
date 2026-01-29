namespace campus_insider.Models
{
    public class Loan
    {
        public long Id { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "PENDING";
        public DateTime CreatedAt { get; set; }

        // Foreign keys
        public long EquipmentId { get; set; }
        public long BorrowerId { get; set; }

        // Navigation
        public Equipment Equipment { get; set; } = null!;
        public User Borrower { get; set; } = null!;
    }

}
