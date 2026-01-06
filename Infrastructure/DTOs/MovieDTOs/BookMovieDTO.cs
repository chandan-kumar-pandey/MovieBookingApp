using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.MovieDTOs
{
    public class BookMovieDTO
    {
        public int MovieId { get; set; }

        public int userId { get; set; }

        public string[] SeatNumbers { get; set; }

    }
}
