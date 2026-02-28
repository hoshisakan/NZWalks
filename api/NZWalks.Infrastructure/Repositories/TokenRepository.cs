using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NZWalks.Application.Repositories;
using NZWalks.Domain.Entities;
using NZWalks.Infrastructure.Data;

namespace NZWalks.Infrastructure.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration _configuration;
        private readonly NZWalksAuthDbContext _authDbContext;
        private readonly ILogger<TokenRepository> _logger;

        public TokenRepository(IConfiguration configuration, NZWalksAuthDbContext authDbContext, ILogger<TokenRepository> logger)
        {
            _configuration = configuration;
            _authDbContext = authDbContext;
            _logger = logger;
        }

        public string CreateJwtToken(IdentityUser user, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName ?? ""),
            };
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> CreateRefreshTokenAsync(IdentityUser user, string jwtId)
        {
            var existingRefreshToken = await _authDbContext.RefreshTokens
                .Where(x => x.UserId == user.Id && !x.IsUsed && !x.IsRevoked)
                .ToListAsync();
            foreach (var token in existingRefreshToken)
                token.IsRevoked = true;

            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString(),
                JwtId = jwtId,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsUsed = false,
                IsRevoked = false
            };
            await _authDbContext.RefreshTokens.AddAsync(refreshToken);
            await _authDbContext.SaveChangesAsync();
            return refreshToken.Token;
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken)
        {
            return await _authDbContext.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken && !x.IsUsed && !x.IsRevoked);
        }

        public async Task MarkRefreshTokenAsUsedAsync(string refreshToken)
        {
            var existing = await _authDbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);
            if (existing != null)
            {
                existing.IsUsed = true;
                existing.IsRevoked = true;
                _logger.LogInformation("Refresh token {Token} marked as used and revoked", refreshToken);
            }
            await _authDbContext.SaveChangesAsync();
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var existing = await _authDbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);
            if (existing != null)
                existing.IsRevoked = true;
            await _authDbContext.SaveChangesAsync();
        }
    }
}
