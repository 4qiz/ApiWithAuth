﻿using AuthWith2Fa.Dtos.Request;
using AuthWith2Fa.Dtos.Response;
using AuthWith2Fa.Entities;
using AuthWith2Fa.JwtFeatures;
using AutoMapper;
using EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace AuthWith2Fa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtHandler _jwt;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<User> userManager, IMapper mapper, JwtHandler jwt, IEmailSender emailSender)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwt = jwt;
            _emailSender = emailSender;
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

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var param = new Dictionary<string, string>
            {
                {"token", token},
                {"email", registrationDto.Email}
            };

            var callback = QueryHelpers.AddQueryString(registrationDto.ClientUri, param);

            var message = new EmailMessage([user.Email], "email confirmation", $"{callback} \n {token}");
            Console.WriteLine("\n" + token + "\n");
            //await _emailSender.SendEmail(message);

            await _userManager.AddToRoleAsync(user, "visitor");

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Email);
            if (user == null)
            {
                return BadRequest();
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Email is not confirmed" });
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Lock" });
            }

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                //lock out
                await _userManager.AccessFailedAsync(user);
                if (await _userManager.IsLockedOutAsync(user))
                {
                    var content = "Your account is locked out";
                    var message = new EmailMessage([user.Email], "account locked out", content);
                    Console.WriteLine(content);
                    //await _emailSender.SendEmail(message);
                    return Unauthorized(new AuthResponseDto { ErrorMessage = "Lock" });
                }
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid" });
            }

            if (await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return await GenerateOtpForTwoFactor(user);
            }

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwt.CreateToken(user, roles);

            await _userManager.ResetAccessFailedCountAsync(user);

            return Ok(new AuthResponseDto { Token = token, IsSuccessful = true });
        }

        private async Task<IActionResult> GenerateOtpForTwoFactor(User user)
        {
            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
            if (!providers.Contains("Email"))
            {
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid 2fa" });
            }

            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            var message = new EmailMessage([user.Email], "2FA token", token);
            Console.WriteLine("\n" + token + "\n");
            //await _emailSender.SendEmail(message);

            return Ok(new AuthResponseDto { Is2FaRequired = true, Provider = "Email" });
        }

        [HttpPost("enable2fa")]
        public async Task<IActionResult> Enable2Fa(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Email);
            if (user == null)
            {
                return BadRequest();
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Email is not confirmed" });
            }

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid" });
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            return Ok();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return BadRequest();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var param = new Dictionary<string, string>
            {
                {"token", token},
                {"email", dto.Email}
            };

            var callback = QueryHelpers.AddQueryString(dto.ClientUri, param);

            var message = new EmailMessage([user.Email], "reset Password", $"{callback} \n {token}");
            Console.WriteLine("\n" + token + "\n");
            //await _emailSender.SendEmail(message);
            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return BadRequest();
            }

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(errors);
            }

            await _userManager.SetLockoutEndDateAsync(user, null);

            return Ok();
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest();
            }

            var confirmResult = await _userManager.ConfirmEmailAsync(user, token);

            if (!confirmResult.Succeeded)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPost("two-factor")]
        public async Task<IActionResult> TwoFactor(TwoFactorDto dto)
        {
            if (!ModelState.IsValid) { return BadRequest(); }
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return BadRequest();
            }
            var validVerification = await _userManager.VerifyTwoFactorTokenAsync(user, dto.Provider, dto.Token);

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwt.CreateToken(user, roles);

            await _userManager.ResetAccessFailedCountAsync(user);

            return Ok(new AuthResponseDto { Token = token, IsSuccessful = true });
        }
    }
}
