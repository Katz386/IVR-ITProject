using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesService.Application.DTOs.Request
{
    public class AddArticleRequest
    {
        [Required(ErrorMessage = "Required field is missing")]
        [Url(ErrorMessage = "Invalid url")]
        public string ImageUrl { get; set; }
        [MaxLength(200, ErrorMessage = "Too long")]
        [Required(ErrorMessage = "Required field is missing")]
        public string Title { get; set; }
        [MaxLength(1000, ErrorMessage = "Too long")]
        [Required(ErrorMessage = "Required field is missing")]
        public string Description { get; set; }
        [MaxLength(400, ErrorMessage = "Too long")]
        [Required(ErrorMessage = "Required field is missing")]
        public string Topic { get; set; }
        [Required(ErrorMessage = "Required field is missing")]
        public string Markdown { get; set; }
        [Required(ErrorMessage = "Required field is missing")]
        public int SortOrder { get; set; }
    }
}
