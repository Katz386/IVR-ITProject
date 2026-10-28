using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<List<WebTaskPreview>?> GetTasks(int page, int pageSize)
        {
            try
            {
                return await _context.Tasks
                    .OrderBy(x => x.SortOrder)
                    .Select(x => new WebTaskPreview
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Difficulty = x.Difficulty,
                        Created = x.Created
                    })
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\"{ex.Message}\" in GetTasks");
                return null;
            }
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
