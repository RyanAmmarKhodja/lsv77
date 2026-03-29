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

        public bool IsEmailVerified { get; set; } = false;
        public string? EmailVerificationToken { get; set; }


        public List<Equipment> Equipment { get; set; } = new();
        public List<Notification> Notifications { get; set; } = new();
        public List<Post> Posts { get; set; } = new();

        public List<ChatParticipant> ChatParticipants { get; set; } = new();
        public List<ChatMessage> SentMessages { get; set; } = new();
    }

}
