using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using Microsoft.AspNetCore.Identity;


namespace NZWalks.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthService(ITokenRepository tokenRepository, UserManager<IdentityUser> userManager)
        {
            _tokenRepository = tokenRepository;
            _userManager = userManager;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.Email);
            
            if (user == null)
            {
                throw new Exception("User not found!");
            }

            var result = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
        
            if (result) {
                var roles = await _userManager.GetRolesAsync(user);
                var jwtTokenId = Guid.NewGuid().ToString();

                if (roles != null && roles.Any())
                {
                    var jwtToken = _tokenRepository.CreateJwtToken(user, roles.ToList());
                
                    var refreshToken = await _tokenRepository.CreateRefreshTokenAsync(user, jwtTokenId);

                    var response = new LoginResponseDto
                    {
                        JWTToken = jwtToken,
                        RefreshToken = refreshToken
                    };

                    return response;
                }
            }
            throw new Exception("Username not found and/or password is incorrect!");
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            var checkUserRoleIncludeAdmin = registerRequestDto.Roles.Contains("Admin");
            
            if (checkUserRoleIncludeAdmin)
            {
                throw new Exception("User cannot be an admin!");
            }

            var user = new IdentityUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Email,
            };

            var identityResult = await _userManager.CreateAsync(user, registerRequestDto.Password);
            if (identityResult.Succeeded)
            {
                if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                {
                    identityResult = await _userManager.AddToRolesAsync(user, registerRequestDto.Roles);
                
                    if (identityResult.Succeeded)
                    {
                        return new RegisterResponseDto
                        {
                            Username = user.UserName,
                            Email = user.Email
                        };
                    }
                }
            }
            throw new Exception("User not created!");
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(TokenRequestDto tokenRequestDto)
        {
            var storedToken = await _tokenRepository.GetRefreshTokenAsync(tokenRequestDto.RefreshToken);
            if (storedToken == null || storedToken.IsRevoked || storedToken.IsUsed || storedToken.ExpiryDate < DateTime.UtcNow)
            {
                throw new Exception("Invalid refresh token!");
            }

            var user = await _userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
            {
                throw new Exception("Invalid refresh token!");
            }

            var roles = await _userManager.GetRolesAsync(user);
            await _tokenRepository.MarkRefreshTokenAsUsedAsync(tokenRequestDto.RefreshToken);
            
            var newJwtToken = _tokenRepository.CreateJwtToken(user, roles.ToList());
            var newRefreshToken = await _tokenRepository.CreateRefreshTokenAsync(user, Guid.NewGuid().ToString());
        
            var response = new LoginResponseDto
            {
                JWTToken = newJwtToken,
                RefreshToken = newRefreshToken
            };

            return response;
        }

        public async Task LogoutAsync(string token)
        {
            await _tokenRepository.RevokeRefreshTokenAsync(token);
        }
    }
}