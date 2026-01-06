using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.AuthDTOs
{
    public class ResetPasswordDTO
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; }
    }
}
