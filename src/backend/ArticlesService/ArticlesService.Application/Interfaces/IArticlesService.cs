using ArticlesService.Application.DTOs.Request;
using ArticlesService.Application.DTOs.Response;
using ArticlesService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesService.Application.Interfaces
{
    public interface IArticlesService
    {
        public Task<GetArticleResponse> GetArticleById(string id);
        public Task<GetArticlePreviewResponse> GetArticlePreviewById(string id);
        public Task<GetArticlesListResponse> GetArticles(int page);
        public Task<DeleteArticleResponse> DeleteArticle(string id);
        public Task<EditArticleResponse> EditArticle(EditArticleRequest request, string id);
        public Task<AddArticleResponse> AddArticle(AddArticleRequest request);
    }
}
