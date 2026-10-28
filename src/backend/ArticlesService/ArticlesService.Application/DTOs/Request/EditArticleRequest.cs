using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesService.Application.DTOs.Request
{
    public class EditArticleRequest
    {
        [Url(ErrorMessage = "Invalid url")]
        public string? ImageUrl { get; set; }
        [MaxLength(200, ErrorMessage = "Too long")]
        public string? Title { get; set; }
        [MaxLength(1000, ErrorMessage = "Too long")]
        public string? Description { get; set; }
        [MaxLength(400, ErrorMessage = "Too long")]
        public string? Topic { get; set; }
        public string? Markdown { get; set; }
        public int? SortOrder { get; set; }
    }
}
