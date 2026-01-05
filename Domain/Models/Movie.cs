using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public partial class Movie
    {
        [Key]
        public int MovieId { get; set; }

        [Required, MaxLength(100)]
        public string MovieName { get; set; }

        [Required]
        public int TotalTickets { get; set; }

        [Required, MaxLength(20)]
        public string TicketStatus { get; set; } // SOLD OUT / BOOK ASAP
    }
}
