using System.ComponentModel.DataAnnotations;

namespace NZWalks.Application.DTO
{
    public class AddDifficultyRequestDto
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Name must be at most 100 characters long")]
        public string Name { get; set; } = string.Empty;
    }
}
