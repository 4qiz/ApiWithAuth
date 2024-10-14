using AuthWith2Fa.Dtos.Request;
using AuthWith2Fa.Dtos.Response;
using AuthWith2Fa.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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

        public AccountController(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserRegistrationDto registrationDto)
        {
            if (registrationDto == null)
            {
                return BadRequest();
            }

            var user = _mapper.Map<User>(registrationDto);

            var result  = await _userManager.CreateAsync(user, registrationDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new RegistrationResponseDto { Errors = errors });
            }
            return StatusCode(201);
        }
    }
}
