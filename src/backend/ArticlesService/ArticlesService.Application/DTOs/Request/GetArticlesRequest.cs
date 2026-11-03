using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesService.Application.DTOs.Request
{
    public enum ArticlesSortBy
    {
        SortOrder,
        Created
    }

    public class GetArticlesRequest
    {
        [Range(1, 100000)]
        public int page { get; set; } = 1;

        [MaxLength(150)]
        public string? search { get; set; }

        [MaxLength(150)]
        public string? topic { get; set; }

        public bool hideRead { get; set; }

        [MaxLength(10000)]
        public string? readArticles { get; set; }

        public ArticlesSortBy sortBy { get; set; } = ArticlesSortBy.Created;

        public bool sortDescending { get; set; } = true;
    }
}
