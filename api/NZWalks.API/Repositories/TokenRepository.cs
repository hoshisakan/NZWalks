using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using NZWalks.API.Models.Domain;
using NZWalks.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace NZWalks.API.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration _configuration;
        private readonly NZWalksAuthDbContext _nZWalksAuthDbContext;
        private readonly ILogger<TokenRepository> _logger;


        public TokenRepository(IConfiguration configuration, NZWalksAuthDbContext nZWalksAuthDbContext, ILogger<TokenRepository> logger)
        {
            _configuration = configuration;
            _nZWalksAuthDbContext = nZWalksAuthDbContext;
            _logger = logger;
        }

        public string CreateJwtToken(IdentityUser user, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
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
            var existingRefreshToken = await _nZWalksAuthDbContext.RefreshTokens
                .Where(x => x.UserId == user.Id && x.IsUsed == false && x.IsRevoked == false)
                .ToListAsync();
            
            if (existingRefreshToken.Any())
            {
                foreach (var token in existingRefreshToken)
                {
                    token.IsRevoked = true;
                }
            }
            
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

            await _nZWalksAuthDbContext.RefreshTokens.AddAsync(refreshToken);

            await _nZWalksAuthDbContext.SaveChangesAsync();

            return refreshToken.Token;
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken)
        {
            return await _nZWalksAuthDbContext.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == refreshToken
                && x.IsUsed == false
                && x.IsRevoked == false
            );
        }

        public async Task MarkRefreshTokenAsUsedAsync(string refreshToken)
        {
            var existingRefreshToken = await _nZWalksAuthDbContext.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken);
            
            if (existingRefreshToken != null)
            {
                existingRefreshToken.IsUsed = true;
                existingRefreshToken.IsRevoked = true;
                _logger.LogInformation($"Refresh token {refreshToken} marked as used");
                _logger.LogInformation($"Refresh token {refreshToken} marked as revoked");
            }

            _logger.LogInformation($"Refresh token {refreshToken} saved to database");

            await _nZWalksAuthDbContext.SaveChangesAsync();
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var existingRefreshToken = await _nZWalksAuthDbContext.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken);
            
            if (existingRefreshToken != null)
            {
                existingRefreshToken.IsRevoked = true;
            }
            
            await _nZWalksAuthDbContext.SaveChangesAsync();
        }
    }
}