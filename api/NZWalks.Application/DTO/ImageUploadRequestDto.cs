using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NZWalks.Application.DTO
{
    public class ImageUploadRequestDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;
        [Required]
        public string FileName { get; set; } = string.Empty;
        public string? FileDescription { get; set; }
    }
}
