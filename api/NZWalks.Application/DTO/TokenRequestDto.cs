using System.ComponentModel.DataAnnotations;

namespace NZWalks.Application.DTO
{
    public class TokenRequestDto
    {
        [Required]
        public string JwtToken { get; set; } = string.Empty;
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
