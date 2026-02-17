namespace campus_insider.DTOs
{
    // Feed wrapper
    public class FeedResponseDto
    {
        public List<FeedItemDto> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }

    // Polymorphic feed item
    public class FeedItemDto
    {
        public string Type { get; set; } = string.Empty; // EQUIPMENT, CARPOOL, POST
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public object Content { get; set; } = null!; // EquipmentDto, CarpoolDto, or PostDto
        public double Priority { get; set; }
    }

   
}