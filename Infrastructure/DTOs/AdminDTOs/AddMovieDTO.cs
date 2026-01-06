using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.AdminDTOs
{
    public class AddMovieDTO
    {
        public string MovieName { get; set; }

        public int TotalTickets { get; set; }

        public string TicketStatus { get; set; }
    }
}
