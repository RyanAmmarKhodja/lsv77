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

        public PostType PostType { get; set; }
        public Category Category { get; set; }
        public bool IsActive { get; set; }
        public int ViewCount { get; set; }

        public DateTime? DepartureTime { get; set; }
        public string? DepartureLocation { get; set; }
        public string? DestinationLocation { get; set; }
        public int? AvailableSeats { get; set; }
        public DateTime? ReturnTime { get; set; }

        public long AuthorId { get; set; }
        public UserResponseDto? Author { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PostCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public PostType PostType { get; set; } = PostType.OFFER;
        public Category Category { get; set; } = Category.AUTRE;
        public string? ImageUrl { get; set; }
    }


    public class PostResponseDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public PostType PostType { get; set; }
        public Category Category { get; set; }

        public DateTime? DepartureTime { get; set; }
        public string? DepartureLocation { get; set; }
        public string? DestinationLocation { get; set; }
        public int? AvailableSeats { get; set; }
        public DateTime? ReturnTime { get; set; }

        public UserResponseDto Author { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }




    //  CORIDE
    public class CorideDto : PostDto
    {
        public new DateTime DepartureTime { get; set; }
        public new string DepartureLocation { get; set; } = string.Empty;
        public new string DestinationLocation { get; set; } = string.Empty;
        public new int AvailableSeats { get; set; }
        public new DateTime? ReturnTime { get; set; }
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
       
    }


   
    public class CreateEquipmentDto : PostCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

    }
}
