using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksService.Domain.Models;

namespace TasksService.Application.Interfaces
{
    public interface ITasksRepository
    {
        public Task<bool> CreateTask(WebTask task);
        public Task<bool> DeleteTask(WebTask task);
        public Task<WebTask?> GetTaskById(int id);
        public Task<List<WebTaskPreview>?> GetTasks(int page, int pageSize);
        public Task<bool> SaveChangesAsync();
    }
}
