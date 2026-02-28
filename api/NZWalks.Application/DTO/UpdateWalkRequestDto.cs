using System.ComponentModel.DataAnnotations;

namespace NZWalks.Application.DTO
{
    public class UpdateWalkRequestDto
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Name must be at most 100 characters long")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(1000, ErrorMessage = "Description must be at most 1000 characters long")]
        public string Description { get; set; } = string.Empty;
        [Required]
        [Range(0, 50, ErrorMessage = "Length must be between 0 and 50")]
        public double LengthInKm { get; set; }
        public string? WalkImageUrl { get; set; }
        [Required(ErrorMessage = "Difficulty ID is required")]
        public Guid DifficultyId { get; set; }
        [Required(ErrorMessage = "Region ID is required")]
        public Guid RegionId { get; set; }
    }
}
