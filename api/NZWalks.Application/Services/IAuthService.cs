using NZWalks.Application.DTO;

namespace NZWalks.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto);
        Task<LoginResponseDto> RefreshTokenAsync(TokenRequestDto tokenRequestDto);
        Task LogoutAsync(string token);
    }
}
