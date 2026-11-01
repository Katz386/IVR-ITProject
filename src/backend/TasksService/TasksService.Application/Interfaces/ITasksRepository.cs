using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksService.Application.DTOs.Request;
using TasksService.Domain.Models;

namespace TasksService.Application.Interfaces
{
    public interface ITasksRepository
    {
        public Task<bool> CreateTask(WebTask task);
        public Task<bool> DeleteTask(WebTask task);
        public Task<WebTask?> GetTaskById(int id);
        Task<List<WebTaskPreview>> GetTasks(GetTasksRequest request, int pageSize);
        public Task<bool> SaveChangesAsync();
    }
}
