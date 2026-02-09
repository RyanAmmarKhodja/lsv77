namespace campus_insider.DTOs
{
    public class CarpoolDto
    {
        public int Id { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public UserResponseDto Driver { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CarpoolCreateDto
    {
        public long driverId { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public string Status { get; set; }
        public int AvailableSeats { get; set; }

    }

    public class CarpoolResponseDto
    {
        public long Id { get; set; }
        public string Departure { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public int AvailableSeats { get; set; }
        public int TotalSeats { get; set; }
        public UserResponseDto? Driver { get; set; }
        public List<UserResponseDto> Passengers { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
