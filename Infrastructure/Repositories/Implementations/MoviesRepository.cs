using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs.CommonDTOs;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations
{
    public class MoviesRepository : IMoviesRepository
    {

        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;
        private readonly ILogger<AuthRepository> _logger;

        public MoviesRepository(AppDbContext context, TokenService tokenService, ILogger<AuthRepository> logger)
        {
            _context = context;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<GeneralApiRespDTO> GetAllMovies()
        {

            try
            {
               
                var movieList = await _context.Movies.ToListAsync();

                if (movieList == null || !movieList.Any())
                {
                    return new GeneralApiRespDTO
                    {
                        Status = 1,
                        Message = "No movies found in the database.",
                        Data = new List<Movie>()
                    };
                }

                return new GeneralApiRespDTO
                {
                    Status = 1,
                    Message = "Movies retrieved successfully.",
                    Data = movieList 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching movies.");

                return new GeneralApiRespDTO
                {
                    Status = 0,
                    Message = "An internal error occurred while retrieving movies."
                };
            }


        }

        public async Task<GeneralApiRespDTO> GetMoviesByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return new GeneralApiRespDTO { Status = 0, Message = "Search term cannot be empty." };
                }

                // Filter movies where the name contains the search string
                var movies = await _context.Movies
                    .Where(m => m.MovieName.ToLower().Contains(name.ToLower()))
                    .ToListAsync();

                if (movies == null || !movies.Any())
                {
                    return new GeneralApiRespDTO
                    {
                        Status = 1,
                        Message = $"No movies found matching: {name}",
                        Data = new List<Movie>()
                    };
                }

                return new GeneralApiRespDTO
                {
                    Status = 1,
                    Message = "Movies retrieved successfully.",
                    Data = movies
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for movies with name: {Name}", name);
                return new GeneralApiRespDTO
                {
                    Status = 0,
                    Message = "An error occurred while searching for movies."
                };
            }
        }

        public async Task<GeneralApiRespDTO> GetMovieById(int id)
        {
            try
            {
                // Use FirstOrDefaultAsync to find the specific movie by its ID
                var movie = await _context.Movies
                    .FirstOrDefaultAsync(m => m.MovieId == id);

                if (movie == null)
                {
                    return new GeneralApiRespDTO
                    {
                        Status = 0,
                        Message = $"Movie with ID {id} was not found.",
                        Data = null
                    };
                }

                return new GeneralApiRespDTO
                {
                    Status = 1,
                    Message = "Movie details retrieved successfully.",
                    Data = movie
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching movie with ID: {Id}", id);
                return new GeneralApiRespDTO
                {
                    Status = 0,
                    Message = "An internal error occurred."
                };
            }
        }
    }
}
