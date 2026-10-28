using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.Application.DTOs.Request
{
    public class SolveTaskRequest
    {
        [MaxLength(500, ErrorMessage = "Too long")]
        public string Flag { get; set; }
    }
}
