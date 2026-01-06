using Infrastructure.DTOs.CommonDTOs;
using Infrastructure.DTOs.MovieDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IMoviesRepository
    {
        Task<GeneralApiRespDTO> GetAllMovies();

        Task<GeneralApiRespDTO> GetMoviesByName(string movieName);

        Task<GeneralApiRespDTO> GetMovieById(int movieId);

        Task<GeneralApiRespDTO> BookMyShow(BookMovieDTO bookMovie);

        Task<GeneralApiRespDTO> GetMyMovieTicket(int userId);

        Task<GeneralApiRespDTO> GetMovieSeatMatrix(int movieId);
    }
}
