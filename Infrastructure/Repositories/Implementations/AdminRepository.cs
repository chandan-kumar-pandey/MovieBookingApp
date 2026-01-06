using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs.AdminDTOs;
using Infrastructure.DTOs.CommonDTOs;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;
        private readonly ILogger<AuthRepository> _logger;

        public AdminRepository(AppDbContext context, ILogger<AuthRepository> logger, TokenService tokenService)
        {
            _context = context;
            _logger = logger;
            _tokenService = tokenService;
        }

        public async Task<GeneralApiRespDTO> AddMovie(AddMovieDTO movie)
        {

            if (movie == null)
            {
                return new GeneralApiRespDTO
                {
                    Status = -1,
                    Message = "Movie Details Needed!"
                };
            }

            if (string.IsNullOrWhiteSpace(movie.MovieName) || movie.TotalTickets <= 0)
            {
                return new GeneralApiRespDTO
                {
                    Status = -1,
                    Message = "Valid Movie Name and Total Tickets are required!"
                };
            }

            try
            {

                var mov = new Movie
                {
                    MovieName = movie.MovieName,
                    TicketStatus = !string.IsNullOrEmpty(movie.TicketStatus) ? movie.TicketStatus : "BOOK_ASAP",
                    TotalTickets = movie.TotalTickets,
                };

                await _context.Movies.AddAsync(mov);

                var changes = await _context.SaveChangesAsync();

                if (changes > 0)
                {
                    _logger.LogInformation("New Movie Added : {@movieId} at {@time}", mov.MovieId, DateTime.UtcNow);

                    return new GeneralApiRespDTO
                    {
                        Status = 1,
                        Message = "Movie Details added successfully.",
                        Id = mov.MovieId

                    };
                }
                else
                {
                    _logger.LogError("Failed to add new movie the database. Movie: {movie}", movie.MovieName);

                    return new GeneralApiRespDTO
                    {
                        Status = 0,
                        Message = "Failed to Add Movie. Please try again."
                    };
                }
            }
            catch (Exception ex)
            {
                return new GeneralApiRespDTO
                {
                    Status = 0,
                    Message = "An error occurred while processing your request."
                };
            }
        }
    }
}
