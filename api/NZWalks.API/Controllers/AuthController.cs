using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using NZWalks.API.Models.DTO;
using NZWalks.API.Services;


namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loginResponse = await _authService.LoginAsync(loginRequestDto);
            
            return Ok(loginResponse);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var registerResponse = await _authService.RegisterAsync(registerRequestDto);

            return Ok(registerResponse);
        }

        [HttpPost]
        [Route("Refresh-Token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequestDto)
        {
            var refreshTokenResponse = await _authService.RefreshTokenAsync(tokenRequestDto);
            
            return Ok(refreshTokenResponse);
        }

        [HttpPost]
        [Route("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] TokenRequestDto tokenRequestDto)
        {
            await _authService.LogoutAsync(tokenRequestDto.RefreshToken);
            
            return Ok("Logged out successfully!");
        }
    }
}