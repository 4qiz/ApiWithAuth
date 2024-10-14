using AuthWith2Fa.Dtos.Request;
using AuthWith2Fa.Dtos.Response;
using AuthWith2Fa.Entities;
using AuthWith2Fa.JwtFeatures;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthWith2Fa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtHandler _jwt;

        public AccountController(UserManager<User> userManager, IMapper mapper, JwtHandler jwt)
        {
            _userManager = userManager;
            _mapper = mapper;
            this._jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserRegistrationDto registrationDto)
        {
            if (registrationDto == null)
            {
                return BadRequest();
            }

            var user = _mapper.Map<User>(registrationDto);

            var result = await _userManager.CreateAsync(user, registrationDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new RegistrationResponseDto { Errors = errors });
            }
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid" });
            }

            var token = _jwt.CreateToken(user);
            return Ok(new AuthResponseDto { Token = token, IsSuccessful = true });
        }
    }
}
