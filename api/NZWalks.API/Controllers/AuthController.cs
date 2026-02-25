using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;


namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenRepository _tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.Email);

            if (user != null)
            {
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

                        return Ok(response);
                    }
                }
            }

            return BadRequest("Username not found and/or password is incorrect!");
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var checkUserRoleIncludeAdmin = registerRequestDto.Roles.Contains("Admin");
            
            if (checkUserRoleIncludeAdmin)
            {
                return BadRequest("Admin user cannot be created through this endpoint!");
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
                        return Ok("User registered successfully!");
                    }
                }
            }

            return BadRequest("Something went wrong!");
        }

        [HttpPost]
        [Route("Refresh-Token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequestDto)
        {
            var storedToken = await _tokenRepository.GetRefreshTokenAsync(tokenRequestDto.RefreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.IsUsed || storedToken.ExpiryDate < DateTime.UtcNow)
            {
                return BadRequest("Invalid refresh token!");
            }

            var user = await _userManager.FindByIdAsync(storedToken.UserId);

            if (user == null)
            {
                return BadRequest("Invalid user!");
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

            return Ok(response);
        }

        [HttpPost]
        [Route("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] TokenRequestDto tokenRequestDto)
        {
            await _tokenRepository.RevokeRefreshTokenAsync(tokenRequestDto.RefreshToken);

            return Ok("Logged out successfully!");
        }
    }
}