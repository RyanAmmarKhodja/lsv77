namespace campus_insider.Models
{
    public class CarpoolTrip
    {
        public long Id { get; set; }

        public string Departure { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public DateTime CreatedAt { get; set; }

        // Foreign key
        public long DriverId { get; set; }

        // Navigation
        public User Driver { get; set; } = null!;
    }

}
