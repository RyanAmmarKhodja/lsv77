namespace campus_insider.Models
{
    public class Notification
    {
        public long Id { get; set; }
        public long UserId { get; set; }

        // Notification content
        public string Type { get; set; } = string.Empty; // LOAN_APPROVED, CARPOOL_JOINED, etc.
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        // State
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }

        // Optional: Link to related entity
        public string? EntityType { get; set; } // "Loan", "CarpoolTrip", "Equipment"
        public long? EntityId { get; set; }

        // Optional: Action button
        public string? ActionUrl { get; set; } // "/api/loans/5/approve"
        public string? ActionText { get; set; } // "Approve Request"

        // Navigation
        public User User { get; set; } = null!;
    }
}