namespace campus_insider.Models
{
    public class Equipment : Post
    {
        public string Location { get; set; } = string.Empty;
        public string? Category { get; set; }
    }
}
