using campus_insider.Models;
using System.ComponentModel.DataAnnotations;

namespace campus_insider.DTOs
{
    // Post DTOs
    public class PostDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string PostType { get; set; }

        public long AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PostCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }


    public class PostResponseDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Category { get; set; } = string.Empty;
       
        public UserResponseDto Author { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }




    //  CORIDE
    public class CorideDto : PostDto
    {
        public DateTime DepartureTime { get; set; }
        public string DepartureLocation { get; set; } = string.Empty;
        public string DestinationLocation { get; set; } = string.Empty;
        public int AvailableSeats { get; set; }
        public bool HasReturnTrip { get; set; }
        public DateTime? ReturnTime { get; set; }
    }
    public class CreateCorideDto : PostCreateDto
    {
        public DateTime DepartureTime { get; set; }

        public string DepartureLocation { get; set; } = string.Empty;

        public string DestinationLocation { get; set; } = string.Empty;

        [Range(1, 10)]
        public int AvailableSeats { get; set; } = 1;

        public DateTime? ReturnTime { get; set; }
    }



    // EQUIPMENT
    public class EquipmentDto : PostDto
    {
        public string Location { get; set; } = string.Empty;
        public string? Category { get; set; }
    }


   
    public class CreateEquipmentDto : PostCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;


        [StringLength(100)]
        public string? Category { get; set; }
    }
}
