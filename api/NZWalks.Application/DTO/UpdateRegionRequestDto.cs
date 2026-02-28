using System.ComponentModel.DataAnnotations;

namespace NZWalks.Application.DTO
{
    public class UpdateRegionRequestDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Code must be at least 3 characters long")]
        [MaxLength(3, ErrorMessage = "Code must be at most 3 characters long")]
        public string Code { get; set; } = string.Empty;
        [Required]
        [MaxLength(100, ErrorMessage = "Name must be at most 100 characters long")]
        public string Name { get; set; } = string.Empty;
        public string? RegionImageUrl { get; set; }
    }
}
