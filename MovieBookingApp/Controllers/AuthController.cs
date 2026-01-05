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
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly TokenService _tokenService;


        public AuthController(IAuthRepository authRepository, TokenService tokenService)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
        }

        [HttpPost("customer/register")]
        public IActionResult RegisterUserAction([FromBody] RegisterUserDTO dto)
        {
            var result = _authRepository.RegisterUser(dto,0);

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

            return BadRequest(new { error = result.Message ?? "Registration failed"});

        }

        [HttpPost("customer/Login")]
        public IActionResult LoginUserAction([FromBody] LoginUserDTO dto)
        {
            var result = _authRepository.Login(dto.Email,dto.Password);

            if (result.Status==1)
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


    }
}
