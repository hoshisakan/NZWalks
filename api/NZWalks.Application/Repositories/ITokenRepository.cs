using Microsoft.AspNetCore.Identity;

namespace NZWalks.Application.Repositories
{
    public interface ITokenRepository
    {
        string CreateJwtToken(IdentityUser user, List<string> roles);
        Task<string> CreateRefreshTokenAsync(IdentityUser user, string jwtId);
        Task<NZWalks.Domain.Entities.RefreshToken?> GetRefreshTokenAsync(string refreshToken);
        Task MarkRefreshTokenAsUsedAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}
