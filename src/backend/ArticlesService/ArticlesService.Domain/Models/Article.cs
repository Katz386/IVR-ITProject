using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesService.Domain.Models
{
    public class Article
    {
        public int ID { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Topic { get; set; }
        public string Markdown { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SortOrder { get; set; }
    }
}
