using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksService.Application.DTOs.Request;
using TasksService.Application.Interfaces;
using TasksService.Domain.Models;
using TasksService.Infrastructure.Persistence;

namespace TasksService.Infrastructure.Repository
{
    public class TasksRepository : ITasksRepository
    {
        private readonly AppDbContext _context;

        public TasksRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateTask(WebTask task)
        {
            try
            {
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\"{ex.Message}\" in CreateTask");
                return false;
            }
        }

        public async Task<bool> DeleteTask(WebTask task)
        {
            try
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\"{ex.Message}\" in DeleteTask");
                return false;
            }
        }

        public async Task<WebTask?> GetTaskById(int id)
        {
            try
            {
                return await _context.Tasks.Where(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\"{ex.Message}\" in GetTaskById");
                return null;
            }
        }

        public async Task<List<WebTaskPreview>?> GetTasks(GetTasksRequest request, int pageSize)
        {
            try
            {
                IQueryable<WebTask> query = _context.Tasks.AsNoTracking();

                if (!string.IsNullOrEmpty(request.search) && !string.IsNullOrWhiteSpace(request.search))
                {
                    string search = request.search.Trim();

                    query = query.Where(x => EF.Functions.ILike(x.Name, $"%{search}%"));
                }

                if (!string.IsNullOrEmpty(request.difficulty) && !string.IsNullOrWhiteSpace(request.difficulty))
                {
                    string difficulty = request.difficulty.ToLower().Trim();

                    query = query.Where(x => x.Difficulty.ToLower().Trim() == difficulty);
                }

                if (!string.IsNullOrEmpty(request.topic) && !string.IsNullOrWhiteSpace(request.topic))
                {
                    string topic = request.topic.ToLower().Trim();

                    query = query.Where(x => x.Topic.ToLower().Trim() == topic);
                }

                IOrderedQueryable<WebTask> orderedQuery;

                switch (request.sortBy)
                {
                    case TaskSortBy.SortOrder:
                        orderedQuery = request.sortDescending ? query.OrderByDescending(x => x.SortOrder) : query.OrderBy(x => x.SortOrder);
                        break;
                    case TaskSortBy.Difficulty:

                        orderedQuery = request.sortDescending
                            ? query.OrderByDescending(x =>
                            x.Difficulty.ToLower().Trim() == "easy" ? 1 :
                            x.Difficulty.ToLower().Trim() == "medium" ? 2 :
                            x.Difficulty.ToLower().Trim() == "hard" ? 3 : 99)
                            : query.OrderBy(x =>
                            x.Difficulty.ToLower().Trim() == "easy" ? 1 :
                            x.Difficulty.ToLower().Trim() == "medium" ? 2 :
                            x.Difficulty.ToLower().Trim() == "hard" ? 3 : 99);

                        break;

                    case TaskSortBy.Created:
                        orderedQuery = request.sortDescending ? query.OrderByDescending(x => x.Created) : query.OrderBy(x => x.Created);
                        break;

                    default:
                        orderedQuery = query.OrderByDescending(x => x.Created);
                        break;
                }

                var selectedQuery = orderedQuery.Select(x =>
                    new WebTaskPreview
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Topic = x.Topic,
                        Difficulty = x.Difficulty,
                        Created = x.Created
                    });

                if (request.hideSolved && !string.IsNullOrEmpty(request.solvedTasks))
                {
                    var tasks = await selectedQuery.ToListAsync();

                    tasks = tasks
                        .Where(x => !IsNumInSequence(request.solvedTasks, x.Id.ToString())).ToList();

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
                Console.WriteLine($"\"{ex.Message}\" in GetTasks");
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
                Console.WriteLine($"\"{ex.Message}\" in SaveChangesAsync");
                return false;
            }
        }
    }
}
