using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

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
    }
}
