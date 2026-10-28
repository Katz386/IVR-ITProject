using ArticlesService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesService.Application.Interfaces
{
    public interface IArticlesRepository
    {
        public Task<Article?> GetArticleById(int id);
        public Task<ArticlePreview?> GetArticlePreviewById(int id);
        public Task<List<ArticlePreview>> GetArticles(int page, int pageSize);
        public Task<bool> AddArticle(Article article);
        public Task<bool> DeleteArticle(Article article);
        public Task<bool> SaveChangesAsync();
    }
}
