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

                    if (roles != null && roles.Any())
                    {
                        var token = _tokenRepository.CreateJwtToken(user, roles.ToList());

                        var response = new LoginResponseDto
                        {
                            JWTToken = token
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
    }
}