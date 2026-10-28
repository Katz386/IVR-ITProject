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

        public async Task<List<ArticlePreview>> GetArticles(int page, int pageSize)
        {
            return await _context.Articles.OrderBy(x => x.SortOrder)
                .Select(x => new ArticlePreview
                {
                    ID = x.ID,
                    ImageUrl = x.ImageUrl,
                    Title = x.Title,
                    Description = x.Description,
                    Topic = x.Topic,
                    CreatedAt = x.CreatedAt
                }).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
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
