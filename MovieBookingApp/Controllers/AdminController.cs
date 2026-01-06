using Domain.Models;
using Infrastructure.DTOs.AuthDTOs;
using Infrastructure.DTOs.AdminDTOs;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;


namespace MovieBookingApp.Controllers
{

    [Route("api/v1.0/moviebooking/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly TokenService _tokenService;
        private readonly IAdminRepository _adminRepository;


        public AdminController(IAdminRepository adminRepository, TokenService tokenService)
        {
            _adminRepository = adminRepository;
            _tokenService = tokenService;
        }

        [Authorize(Roles ="Admin")]
        [HttpPost("addmovie")]
        public async Task<IActionResult> AddMovieAction([FromBody] AddMovieDTO dto)
        {
            if (dto is null)
            {
                return BadRequest(new { error = "Request body cannot be null." });
            }

            // Call repository asynchronously
            var result = await _adminRepository.AddMovie(dto);

            if (result is null)
            {
                return BadRequest(new { error = "Adding Movie failed." });
            }

            if (result.Status > 0)
            {
                return Ok(new
                {
                    msg = "Movie Added Successfully!!",
                    result.Id,
                    result.Status
                });
            }

            return BadRequest(new { error = result.Message ?? "Adding Movie failed" });
        }
    }
}
