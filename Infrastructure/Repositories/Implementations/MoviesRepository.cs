using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs.CommonDTOs;
using Infrastructure.DTOs.MovieDTOs;
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

        public async Task<GeneralApiRespDTO> BookMyShow(BookMovieDTO bookMovie)
        {
            // Use a transaction to ensure all seats are booked or none are (Atomic operation)
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Basic Validation
                if (bookMovie.SeatNumbers == null || bookMovie.SeatNumbers.Length == 0)
                {
                    return new GeneralApiRespDTO { Status = 0, Message = "No seats selected." };
                }

                // 2. Fetch Movie and Check Inventory
                var movie = await _context.Movies.FirstOrDefaultAsync(m => m.MovieId == bookMovie.MovieId);
                if (movie == null)
                {
                    return new GeneralApiRespDTO { Status = 0, Message = "Movie not found." };
                }

                int requestedCount = bookMovie.SeatNumbers.Length;

                if (movie.TotalTickets < requestedCount)
                {
                    return new GeneralApiRespDTO
                    {
                        Status = 0,
                        Message = $"Not enough tickets available. Remaining: {movie.TotalTickets}"
                    };
                }

                // 3. CHECK FOR DUPLICATES (Already Booked)
                // Check if ANY of the requested seats are already in the database for this movie
                var alreadyBookedSeats = await _context.Tickets
                    .Where(t => t.MovieId == bookMovie.MovieId && bookMovie.SeatNumbers.Contains(t.SeatNumber))
                    .Select(t => t.SeatNumber)
                    .ToListAsync();

                if (alreadyBookedSeats.Any())
                {
                    return new GeneralApiRespDTO
                    {
                        Status = 0,
                        Message = $"The following seats are already taken: {string.Join(", ", alreadyBookedSeats)}"
                    };
                }

                // 4. Create Ticket Records for EACH seat
                var ticketsToCreate = new List<Ticket>();
                foreach (var seat in bookMovie.SeatNumbers)
                {
                    ticketsToCreate.Add(new Ticket
                    {
                        MovieId = bookMovie.MovieId,
                        UserId = bookMovie.userId,
                        SeatNumber = seat // The string from Angular (e.g., "A1")
                    });
                }

                _context.Tickets.AddRange(ticketsToCreate);

                // 5. Update Movie Inventory by the total count
                movie.TotalTickets -= requestedCount;

                if (movie.TotalTickets <= 0)
                {
                    movie.TotalTickets = 0;
                    movie.TicketStatus = "SOLD OUT";
                }
                else if (movie.TotalTickets < 10)
                {
                    movie.TicketStatus = "BOOK ASAP";
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralApiRespDTO
                {
                    Status = 1,
                    Message = $"Success! {requestedCount} seats reserved: {string.Join(", ", bookMovie.SeatNumbers)}",
                    Data = ticketsToCreate
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Booking failed for MovieId: {MovieId}", bookMovie.MovieId);
                return new GeneralApiRespDTO { Status = 0, Message = "An internal error occurred." };
            }
        }

        private string MapSeatToRow(int seatNumber)
        {
            char row = seatNumber switch
            {
                <= 20 => 'A',
                <= 40 => 'B',
                <= 60 => 'C',
                <= 80 => 'D',
                _ => 'E'
            };
            return $"{row}{seatNumber}";
        }

        public async Task<GeneralApiRespDTO> GetMyMovieTicket(int userId)
        {
            try
            {
                // Left join tickets with movies to include movie name (if available)
                var ticketsWithMovie = await (
                    from t in _context.Tickets
                    where t.UserId == userId
                    join m in _context.Movies on t.MovieId equals m.MovieId into movieGroup
                    from m in movieGroup.DefaultIfEmpty()
                    select new
                    {
                        // Preserve ticket fields; include MovieName from joined movie (may be null)
                        TicketId = EF.Property<int?>(t, "TicketId"), // safe access if TicketId exists; nullable fallback
                        t.MovieId,
                        t.UserId,
                        t.SeatNumber,
                        MovieName = m != null ? m.MovieName : null
                    }
                ).ToListAsync();

                if (ticketsWithMovie == null || !ticketsWithMovie.Any())
                {
                    return new GeneralApiRespDTO
                    {
                        Status = 1,
                        Message = "No tickets found for this user.",
                        Data = new List<object>()
                    };
                }

                return new GeneralApiRespDTO
                {
                    Status = 1,
                    Message = "Tickets retrieved successfully.",
                    Data = ticketsWithMovie
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tickets for UserId: {UserId}", userId);
                return new GeneralApiRespDTO
                {
                    Status = 0,
                    Message = "An error occurred while retrieving tickets."
                };
            }
        }

        public async Task<GeneralApiRespDTO> GetMovieSeatMatrix(int movieId)
        {
            try
            {
                var movie = await _context.Movies.FirstOrDefaultAsync(m => m.MovieId == movieId);
                if (movie == null) return new GeneralApiRespDTO { Status = 0, Message = "Movie not found" };

                var bookedSeats = await _context.Tickets
                    .Where(t => t.MovieId == movieId)
                    .Select(t => t.SeatNumber)
                    .ToListAsync();

                int totalCapacity = movie.TotalTickets + bookedSeats.Count;

                // Generate names like A1, A2... B21, B22...
                var allSeats = new List<string>();
                for (int i = 1; i <= totalCapacity; i++)
                {
                    // Calculate Row Letter: (i-1)/20. 0=A, 1=B, 2=C...
                    char rowLetter = (char)('A' + (i - 1) / 20);
                    allSeats.Add($"{rowLetter}{i}");
                }

                var availableSeats = allSeats.Except(bookedSeats).ToList();

                return new GeneralApiRespDTO
                {
                    Status = 1,
                    Message = "Seat matrix retrieved successfully.",
                    Data = new
                    {
                        MovieId = movie.MovieId,
                        MovieName = movie.MovieName,
                        OriginalSeatNumbers = allSeats,
                        BookedSeatNumbers = bookedSeats,
                        AvailableSeatNumbers = availableSeats
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching seat matrix");
                return new GeneralApiRespDTO { Status = 0, Message = "An error occurred." };
            }
        }
    }
}
