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

        /// <summary>
        /// Optional payload that can hold any type of data. Null when there is no additional data.
        /// </summary>
        public object? Data { get; set; } = null;
    }
}
