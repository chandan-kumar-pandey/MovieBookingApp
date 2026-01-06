using Domain.Models;
using Infrastructure.DTOs.AuthDTOs;
using Infrastructure.Repositories.Interfaces;
//using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
//using Infrastructure.DTOs.UserDTOs;

namespace MovieBookingApp.Controllers
{
    [Route("api/v1.0/moviebooking/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly TokenService _tokenService;


        public AuthController(IAuthRepository authRepository, TokenService tokenService)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public IActionResult RegisterUserAction([FromBody] RegisterUserDTO dto)
        {
            var result = _authRepository.RegisterUser(dto, 0);

            if (result.Status > 0)
            {
                return Ok(new
                {
                    msg = "Registration Successful!!",
                    result.Id,
                    result.Status
                });
            }

            //throw new ArgumentException("User with this email or phone already exists.");

            return BadRequest(new { error = result.Message ?? "Registration failed" });

        }

        [HttpPost("login")]
        public IActionResult LoginUserAction([FromBody] LoginUserDTO dto)
        {
            var result = _authRepository.Login(dto.LoginId, dto.Password);

            if (result.Status == 1)
            {
                return Ok(new
                {
                    msg = !string.IsNullOrEmpty(result.Message) ? result.Message : "Login Successful!!",
                    result
                });
            }

            //throw new ArgumentException("User with this email or phone already exists.");

            return Unauthorized("Invalid credentials.");

        }

        [Authorize]
        [HttpGet("{username}/forgot")]
        public IActionResult ForgotPassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username is required.");
            }

            var message = _authRepository.ForgotPassword(username);

            if (message == "User not found.")
            {
                return NotFound(message);
            }

            return Ok(new { msg = message });
        }

        [Authorize]
        [HttpPut("reset-password")]

        public IActionResult ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            var result = _authRepository.ResetPassword(dto.UserId, dto.NewPassword);
            if (result.Status > 0)
            {
                return Ok(new
                {
                    msg = !string.IsNullOrEmpty(result.Message) ? result.Message : "Password reset successful.",
                    result.Status
                });
            }
            return BadRequest(new { error = result.Message ?? "Password reset failed." });
        }
    }
}
