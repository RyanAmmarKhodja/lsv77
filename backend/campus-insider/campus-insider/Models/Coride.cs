namespace campus_insider.Models;

public class Coride : Post
{
    public DateTime DepartureTime { get; set; }
    public string DepartureLocation { get; set; } = string.Empty;
    public string DestinationLocation { get; set; } = string.Empty;
    public int AvailableSeats { get; set; }
    public DateTime? ReturnTime { get; set; }
}