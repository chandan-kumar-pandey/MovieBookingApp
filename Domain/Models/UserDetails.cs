using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models;

public partial class UserDetails
{
    [Key]
    public int UserId { get; set; }

    [Required, MaxLength(50)]
    public string LoginID { get; set; }

    [Required, MaxLength(50)]
    public string FirstName { get; set; }

    [Required, MaxLength(50)]
    public string LastName { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required, MaxLength(15)]
    public string ContactNumber { get; set; }

    /// <summary>
    /// 0 = Customer, 1 = Admin
    /// </summary>
    [Required]
    public int UserType { get; set; }
}
