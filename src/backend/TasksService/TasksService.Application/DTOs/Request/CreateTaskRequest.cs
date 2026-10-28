using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TasksService.Application.DTOs.Request
{
    public class CreateTaskRequest
    {
        [MaxLength(150, ErrorMessage = "Too long")]
        [Required(ErrorMessage = "Required field is missing")]
        public string Name { get; set; }
        [MaxLength(1000, ErrorMessage = "Too long")]
        [Required(ErrorMessage = "Required field is missing")]
        public string Description { get; set; }
        [MaxLength(500, ErrorMessage = "Too long")]
        [Required(ErrorMessage = "Required field is missing")]
        public string Flag { get; set; }
        [MaxLength(1000, ErrorMessage = "Too long")]
        [Required(ErrorMessage = "Required field is missing")]
        public string File { get; set; }
        [MaxLength(200, ErrorMessage = "Too long")]
        [Required(ErrorMessage = "Required field is missing")]
        public string Difficulty { get; set; }
        [Required(ErrorMessage = "Required field is missing")]
        public string[] Hints { get; set; }
        [MaxLength(10000, ErrorMessage = "Too long")]
        [Required(ErrorMessage = "Required field is missing")]
        public string Solution { get; set; }
        [Required(ErrorMessage = "Required field is missing")]
        public int SortOrder { get; set; }
    }
}
