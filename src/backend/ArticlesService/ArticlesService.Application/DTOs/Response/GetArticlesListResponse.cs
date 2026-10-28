using ArticlesService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ArticlesService.Application.DTOs.Response
{
    public class GetArticlesListResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<ArticlePreview>? Articles { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
    }
}
