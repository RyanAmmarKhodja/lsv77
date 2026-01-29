namespace campus_insider.Models
{
    public class User
    {
        public long Id { get; set; }

        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Role { get; set; } = "USER";
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<CarpoolTrip> CarpoolTrips { get; set; } = new List<CarpoolTrip>();
        public ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

}
