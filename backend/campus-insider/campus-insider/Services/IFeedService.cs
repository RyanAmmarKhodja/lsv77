using campus_insider.DTOs;

namespace campus_insider.Services
{
    public interface IFeedService
    {
        Task<IEnumerable<PostDto>> GetFeedAsync(string? type = null, int page = 1, int pageSize = 20);
        Task<IEnumerable<PostDto>> SearchPostsAsync(string searchTerm, string? type = null);
        Task<IEnumerable<CorideDto>> GetUpcomingCoridesAsync(string? location = null);
        Task<IEnumerable<EquipmentDto>> GetAvailableEquipmentAsync(string? category = null, string? location = null);
    }
}
