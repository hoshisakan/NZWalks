using System.ComponentModel.DataAnnotations;

namespace NZWalks.Application.DTO
{
    public class RegisterRequestDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public string[]? Roles { get; set; }
    }
}
