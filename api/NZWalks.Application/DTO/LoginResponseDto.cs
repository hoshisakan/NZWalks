namespace NZWalks.Application.DTO
{
    public class LoginResponseDto
    {
        public string JWTToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
