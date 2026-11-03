using ArticlesService.Application.DTOs.Request;
using ArticlesService.Application.Interfaces;
using ArticlesService.Domain.Models;
using ArticlesService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesService.Infrastructure.Repository
{
    public class ArticlesRepository : IArticlesRepository
    {
        private readonly AppDbContext _context;

        public ArticlesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddArticle(Article article)
        {
            try
            {
                _context.Articles.Add(article);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> DeleteArticle(Article article)
        {
            try
            {
                if (article != null)
                    _context.Articles.Remove(article);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Article?> GetArticleById(int id)
        {
            return await _context.Articles.Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task<ArticlePreview?> GetArticlePreviewById(int id)
        {
            return await _context.Articles.AsNoTracking()
                .Select(x => new ArticlePreview
                {
                    ID = x.ID,
                    ImageUrl = x.ImageUrl,
                    Title = x.Title,
                    Description = x.Description,
                    Topic = x.Topic,
                    CreatedAt = x.CreatedAt
                }).Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task<List<ArticlePreview>> GetArticles(GetArticlesRequest request, int pageSize)
        {
            try
            {
                IQueryable<Article> query = _context.Articles.AsNoTracking();

                if (!string.IsNullOrEmpty(request.search) && !string.IsNullOrWhiteSpace(request.search))
                {
                    string search = request.search.Trim();

                    query = query.Where(x => EF.Functions.ILike(x.Title, $"%{search}%"));
                }

                if (!string.IsNullOrEmpty(request.topic) && !string.IsNullOrWhiteSpace(request.topic))
                {
                    string topic = request.topic.ToLower().Trim();

                    query = query.Where(x => x.Topic.ToLower().Trim() == topic);
                }

                IOrderedQueryable<Article> orderedQuery;

                switch (request.sortBy)
                {
                    case ArticlesSortBy.SortOrder:
                        orderedQuery = request.sortDescending ? query.OrderByDescending(x => x.SortOrder) : query.OrderBy(x => x.SortOrder);
                        break;
                    case ArticlesSortBy.Created:
                        orderedQuery = request.sortDescending ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt);
                        break;

                    default:
                        orderedQuery = query.OrderByDescending(x => x.CreatedAt);
                        break;
                }

                var selectedQuery = orderedQuery.Select(x =>
                    new ArticlePreview
                    {
                        ID = x.ID,
                        ImageUrl = x.ImageUrl,
                        Title = x.Title,
                        Description = x.Description,
                        Topic = x.Topic,
                        CreatedAt = x.CreatedAt
                    });

                if (request.hideRead && !string.IsNullOrEmpty(request.readArticles))
                {
                    var tasks = await selectedQuery.ToListAsync();

                    tasks = tasks
                        .Where(x => !IsNumInSequence(request.readArticles, x.ID.ToString())).ToList();

                    return tasks
                        .Skip((request.page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                }

                return await selectedQuery
                    .Skip((request.page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\"{ex.Message}\" in GetArticles");
                return null;
            }
        }


        private static bool IsNumInSequence(string s, string num)
        {
            int n = int.Parse(num);

            foreach (string part in s.Split(","))
            {
                int k = part.IndexOf('-');

                if (k == -1)
                {
                    if (part == num)
                        return true;
                }
                else
                {
                    int x = int.Parse(part.Substring(0, k));
                    int y = int.Parse(part.Substring(k + 1));

                    if (n >= x && n <= y)
                        return true;
                }
            }

            return false;
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
