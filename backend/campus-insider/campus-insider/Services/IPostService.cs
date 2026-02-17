using campus_insider.DTOs;

namespace campus_insider.Services
{
    public interface IPostService
    {
        // Create
        Task<CorideDto> CreateCorideAsync(long authorId, CreateCorideDto dto);
        Task<EquipmentDto> CreateEquipmentAsync(long authorId, CreateEquipmentDto dto);

        // Read
        Task<PostDto?> GetPostByIdAsync(long id);
        //Task<IEnumerable<PostDto>> GetUserPostsAsync(long userId);


        // Delete
        Task<bool> DeletePostAsync(long id, long authorId);

        // Deactivate (soft delete)
        Task<bool> DeactivatePostAsync(long id, long authorId);
    }
}
