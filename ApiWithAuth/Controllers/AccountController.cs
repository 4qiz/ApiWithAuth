﻿using ApiWithAuth.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiWithAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var user = new IdentityUser { UserName = registerDto.Username, Email = registerDto.Email };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            return result.Succeeded ? Ok(new { message = "Success" }) : BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:Key"]!)),
                    claims: authClaims,
                    signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)), SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            return Unauthorized();
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole(string role)
        {
            if (await _roleManager.RoleExistsAsync(role))
            {
                return BadRequest("Role exists");
            }
            var result = await _roleManager.CreateAsync(new IdentityRole(role));
            if (result.Succeeded)
            {
                return Ok(new { Message = "Role Added" });
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(UserRoleDto roleDto)
        {
            var user = await _userManager.FindByNameAsync(roleDto.Username);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.AddToRoleAsync(user, roleDto.Role);

            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }
    }
}
