using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NZWalks.API.Models.Domain;


namespace NZWalks.API.Repositories
{
    public interface ITokenRepository
    {
        public string CreateJwtToken(IdentityUser user, List<string> roles);
        Task<string> CreateRefreshTokenAsync(IdentityUser user, string jwtId);
        Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
        Task MarkRefreshTokenAsUsedAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}