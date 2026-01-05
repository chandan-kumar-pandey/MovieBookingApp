using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models;

public partial class Ticket
{
    [Key]
    public int TicketId { get; set; }

    [Required]
    public int MovieId { get; set; }

    [ForeignKey(nameof(MovieId))]
    public Movie Movie { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public UserDetails User { get; set; }

    [Required, MaxLength(50)]
    public string SeatNumber { get; set; }
}
