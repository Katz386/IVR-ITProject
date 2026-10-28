using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.Application.DTOs.Request
{
    public class EditTaskRequest
    {
        [MaxLength(150, ErrorMessage = "Too long")]
        public string? Name { get; set; }
        [MaxLength(1000, ErrorMessage = "Too long")]
        public string? Description { get; set; }
        [MaxLength(500, ErrorMessage = "Too long")]
        public string? Flag { get; set; }
        [MaxLength(1000, ErrorMessage = "Too long")]
        public string? File { get; set; }
        [MaxLength(200, ErrorMessage = "Too long")]
        public string? Difficulty { get; set; }
        public string[]? Hints { get; set; }
        [MaxLength(10000, ErrorMessage = "Too long")]
        public string? Solution { get; set; }
        public int? SortOrder { get; set; }
    }
}
