using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksService.Application.DTOs.Request;
using TasksService.Application.DTOs.Response;

namespace TasksService.Application.Interfaces
{
    public interface ITasksService
    {
        public Task<CreateTaskResponse> CreateTask(CreateTaskRequest request);
        public Task<DeleteTaskResponse> DeleteTask(string id);
        public Task<EditTaskResponse> EditTask(EditTaskRequest request, string id);
        public Task<GetTaskResponse> GetTask(string id);
        public Task<GetTasksListResponse> GetTasks(int page);
        public Task<SolveTaskResponse> SolveTask(string id, string flag);
        public Task<SolveTaskResponse> SolveTask(string id, string taskId, string flag);
    }
}
