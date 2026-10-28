using ArticlesService.Application.DTOs.Request;
using ArticlesService.Application.DTOs.Response;
using ArticlesService.Application.Interfaces;
using ArticlesService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesService.Application.Services
{
    public class ArticlesService : IArticlesService
    {
        private readonly IArticlesRepository _articlesRepository;

        public ArticlesService (IArticlesRepository articlesRepository)
        {
            _articlesRepository = articlesRepository;
        }

        public async Task<AddArticleResponse> AddArticle(AddArticleRequest request)
        {
            Article? article;

            article = new Article
            {
                ImageUrl = request.ImageUrl,
                Title = request.Title,
                Description = request.Description,
                Topic = request.Topic,
                Markdown = request.Markdown,
                CreatedAt = DateTime.UtcNow,
                SortOrder = request.SortOrder
            };

            if (await _articlesRepository.AddArticle(article))
                return new AddArticleResponse { Article = article, Message = "Success", Status = 201};
            else
                return new AddArticleResponse { Message = "Unknown internal error", Status = 500 };
        }

        public async Task<DeleteArticleResponse> DeleteArticle(string id)
        {
            int articleId;

            if (string.IsNullOrEmpty(id) || !Int32.TryParse(id, out articleId) || articleId >= 20000000 || articleId <= -1)
                return new DeleteArticleResponse { Message = "Invalid article id", Status = 400 };

            var article = await _articlesRepository.GetArticleById(articleId);

            if (article == null)
                return new DeleteArticleResponse { Message = "Article not found", Status = 404 };

            if (await _articlesRepository.DeleteArticle(article))
                return new DeleteArticleResponse { Message = "Success", Status = 200 };
            else
                return new DeleteArticleResponse { Message = "Unknown internal error", Status = 500 };

        }

        public async Task<EditArticleResponse> EditArticle(EditArticleRequest request, string id)
        {
            int articleId;

            if (string.IsNullOrEmpty(id) || !Int32.TryParse(id, out articleId) || articleId >= 20000000 || articleId <= -1)
                return new EditArticleResponse { Message = "Invalid article id", Status = 400 };

            var article = await _articlesRepository.GetArticleById(articleId);

            if (article == null)
                return new EditArticleResponse { Message = "Article not found", Status = 404 };

            try
            {
                request.ImageUrl ??= article.ImageUrl;
                request.Title ??= article.Title;
                request.Description ??= article.Description;
                request.Topic ??= article.Topic;
                request.Markdown ??= article.Markdown;
                request.SortOrder ??= article.SortOrder;

                article.ImageUrl = request.ImageUrl;
                article.Title = request.Title;
                article.Description = request.Description;
                article.Topic = request.Topic;
                article.Markdown = request.Markdown;
                article.SortOrder = (int)request.SortOrder;

                await _articlesRepository.SaveChangesAsync();

                return new EditArticleResponse { Article  = article, Message = "Success", Status = 200 };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new EditArticleResponse { Message = "Unknown internal error in EditArticle", Status = 500 };
            }
        }

        public async Task<GetArticleResponse> GetArticleById(string id)
        {
            int articleId;

            if (string.IsNullOrEmpty(id) || !Int32.TryParse(id, out articleId) || articleId >= 20000000 || articleId <= -1)
                return new GetArticleResponse { Message = "Invalid article id", Status = 400 };

            var article = await _articlesRepository.GetArticleById(articleId);

            if (article == null)
                return new GetArticleResponse{ Message = "Article not found", Status = 404};

            return new GetArticleResponse { Article = article, Message = "Success", Status = 200 };
        }

        public async Task<GetArticlePreviewResponse> GetArticlePreviewById(string id)
        {
            int articleId;

            if (string.IsNullOrEmpty(id) || !Int32.TryParse(id, out articleId) || articleId >= 20000000 || articleId <= -1)
                return new GetArticlePreviewResponse { Message = "Invalid article id", Status = 400 };

            var article = await _articlesRepository.GetArticlePreviewById(articleId);

            if (article == null)
                return new GetArticlePreviewResponse { Message = "Article not found", Status = 404 };

            return new GetArticlePreviewResponse { Article = article, Message = "Success", Status = 200 };
        }

        public async Task<GetArticlesListResponse> GetArticles(int page)
        {
            try
            {
                int pageSize = 16;

                if (page <= 0 || page > 4000)
                    return new GetArticlesListResponse { Message = "Invalid article id", Status = 400 };

                var articles = await _articlesRepository.GetArticles(page, pageSize);

                if (articles == null || articles.Count <= 0)
                    return new GetArticlesListResponse { Message = "No articles mathcing the request", Status = 404 };

                return new GetArticlesListResponse { Articles = articles, Message = "Success", Status = 200 };

            }
            catch
            {
                Console.WriteLine("Internal error during GetArticles");
                return new GetArticlesListResponse { Message = "Internal error", Status = 500 };
            }
        }
    }
}
