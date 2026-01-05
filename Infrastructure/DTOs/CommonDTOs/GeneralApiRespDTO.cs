using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.CommonDTOs
{
    public class GeneralApiRespDTO
    {

        public int Status { get; set; }
        public string Message { get; set; }
        public int? Id { get; set; }
    }
}
