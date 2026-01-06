using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs.AdminDTOs;
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

        public async Task<GeneralApiRespDTO> RemoveMovie(int movieId)
        {
            // Start a transaction to ensure both deletions succeed or both fail
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Find the movie
                var movie = await _context.Movies.FirstOrDefaultAsync(m => m.MovieId == movieId);

                if (movie == null)
                {
                    return new GeneralApiRespDTO { Status = 0, Message = "Movie not found." };
                }

                // 2. Find and Remove all related Tickets
                var relatedTickets = _context.Tickets.Where(t => t.MovieId == movieId);
                if (await relatedTickets.AnyAsync())
                {
                    _context.Tickets.RemoveRange(relatedTickets);
                }

                // 3. Remove the movie
                _context.Movies.Remove(movie);

                // 4. Save changes and commit transaction
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralApiRespDTO
                {
                    Status = 1,
                    Message = "Movie and all associated tickets removed successfully.",
                    Id = movieId
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Undo changes if something goes wrong
                _logger.LogError(ex, "Error deleting movie and tickets for ID: {Id}", movieId);

                return new GeneralApiRespDTO
                {
                    Status = 0,
                    Message = "An error occurred during deletion. No data was removed."
                };
            }
        }

        public async Task<GeneralApiRespDTO> UpdateMovieTicketStatus(UpdateMovieDTO mov)
        {
            try
            {
                // 1. Validate the input
                if (mov == null || mov.MovieId <= 0)
                {
                    return new GeneralApiRespDTO { Status = 0, Message = "Invalid movie data provided." };
                }

                // 2. Find the movie in the database
                var movie = await _context.Movies.FirstOrDefaultAsync(m => m.MovieId == mov.MovieId);

                if (movie == null)
                {
                    return new GeneralApiRespDTO
                    {
                        Status = 0,
                        Message = $"Movie with ID {mov.MovieId} not found."
                    };
                }

              
                movie.TicketStatus = mov.TicketStatus;

                // 4. Save the changes
                _context.Movies.Update(movie);
                await _context.SaveChangesAsync();

                return new GeneralApiRespDTO
                {
                    Status = 1,
                    Message = "Ticket status updated successfully.",
                    Data = movie
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ticket status for Movie ID: {Id}", mov.MovieId);
                return new GeneralApiRespDTO
                {
                    Status = 0,
                    Message = "An error occurred while updating ticket status."
                };
            }
        }
    }
}
