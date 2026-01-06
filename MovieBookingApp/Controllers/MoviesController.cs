using Infrastructure.DTOs.MovieDTOs;
using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MovieBookingApp.Controllers
{
    [Route("api/v1.0/moviebooking/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly IMoviesRepository _movieRepository;


        public MoviesController(IMoviesRepository movieRepository, TokenService tokenService)
        {
            _movieRepository = movieRepository;
            _tokenService = tokenService;
        }

        [HttpGet("getallmovies")]
        public async Task<IActionResult> GetAllMoviesAction()
        {
            var result = await _movieRepository.GetAllMovies();
            if (result is null)
            {
                return BadRequest(new { error = "Fetching Movies failed." });
            }
            if (result.Status > 0)
            {
                return Ok(new
                {
                    msg = "Movies Fetched Successfully!!",
                    result.Data,
                    result.Status
                });
            }
            return BadRequest(new { error = result.Message ?? "Fetching Movies failed" });
        }

        [HttpGet("movie-by-name/{movieName}")]
        public async Task<IActionResult> GetMovieDetailsByNameAction(string movieName)
        {
            var result = await _movieRepository.GetMoviesByName(movieName);
            if (result is null)
            {
                return BadRequest(new { error = "Fetching Movie Details failed." });
            }
            if (result.Status > 0)
            {
                return Ok(new
                {
                    msg = "Movie Details Fetched Successfully!!",
                    result.Data,
                    result.Status
                });
            }
            return BadRequest(new { error = result.Message ?? "Fetching Movie Details failed" });
        }

        [HttpGet("movie-by-id/{movieId}")]
        public async Task<IActionResult> GetMovieDetailsByIdAction(int movieId)
        {
            var result = await _movieRepository.GetMovieById(movieId);
            if (result is null)
            {
                return BadRequest(new { error = "Fetching Movie Details failed." });
            }
            if (result.Status > 0)
            {
                return Ok(new
                {
                    msg = "Movie Details Fetched Successfully!!",
                    result.Data,
                    result.Status
                });
            }
            return BadRequest(new { error = result.Message ?? "Fetching Movie Details failed" });



        }

        [Authorize(Roles = "Customer")] // Only logged-in users can book
        [HttpPost("book")]
        public async Task<IActionResult> BookTicket([FromBody] BookMovieDTO bookMovieDto)
        {
            // 1. Security Check: Validate that the logged-in user matches the userId in the DTO
            var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdFromToken) || userIdFromToken != bookMovieDto.userId.ToString())
            {
                return Forbid("You can only book tickets for your own account.");
            }

            // 2. Call the repository logic
            var result = await _movieRepository.BookMyShow(bookMovieDto);

            // 3. Return appropriate response
            if (result.Status == 1)
            {
                return Ok(result); // Returns 200 OK with success message and seat info
            }

            // Return 400 Bad Request for business logic failures (Sold out, Seat taken, etc.)
            return BadRequest(result);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("tickets/{userId}")]
        public async Task<IActionResult> FetchTickets(int userId)
        {
            // 1. Security Check: Extract the UserId from the logged-in user's Token
            var userIdFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // 2. Prevent users from viewing other people's tickets
            if (string.IsNullOrEmpty(userIdFromToken) || userIdFromToken != userId.ToString())
            {
                return Forbid("Access Denied: You can only view your own tickets.");
            }

            // 3. Call the Repository
            var result = await _movieRepository.GetMyMovieTicket(userId);

            if (result.Status == 1)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("seats-matrix/{movieId}")]
        public async Task<IActionResult> GetSeatsMatrix(int movieId)
        {
            var result = await _movieRepository.GetMovieSeatMatrix(movieId);
            if (result is null)
            {
                return BadRequest(new { error = "Fetching Seats Matrix failed." });
            }
            if (result.Status > 0)
            {
                return Ok(new
                {
                    msg = "Seats Matrix Fetched Successfully!!",
                    result.Data,
                    result.Status
                });
            }
            return BadRequest(new { error = result.Message ?? "Fetching Seats Matrix failed" });
        }
    }
}
