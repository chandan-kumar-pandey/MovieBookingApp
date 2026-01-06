using Infrastructure.DTOs.AdminDTOs;
using Infrastructure.DTOs.CommonDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<GeneralApiRespDTO> AddMovie(AddMovieDTO movie);

        Task<GeneralApiRespDTO> RemoveMovie(int movieId);

        Task<GeneralApiRespDTO> UpdateMovieTicketStatus(UpdateMovieDTO movie);
    }
}
