using SharedContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksService.Application.DTOs.Request;
using TasksService.Application.DTOs.Response;
using TasksService.Application.Interfaces;
using TasksService.Domain.Models;

namespace TasksService.Application.Service
{
    public class TasksService : ITasksService
    {
        private readonly ITasksRepository _tasksRepository;
        private readonly IServiceBus _serviceBus;

        public TasksService (ITasksRepository tasksRepository, IServiceBus serviceBus)
        {
            _tasksRepository = tasksRepository;
            _serviceBus = serviceBus;
        }

        public async Task<CreateTaskResponse> CreateTask(CreateTaskRequest request)
        {

            WebTask task = new WebTask
            {
                Name = request.Name,
                Description = request.Description,
                Flag = request.Flag,
                File = request.File,
                Difficulty = request.Difficulty,
                Hints = request.Hints,
                Solution = request.Solution,
                Created = DateTime.UtcNow,
                SortOrder = request.SortOrder
            };

            if (await _tasksRepository.CreateTask(task))
            {
                return new CreateTaskResponse { Task = task, Message = "Success", Status = 201 };
            }
            else
            {
                return new CreateTaskResponse { Message = "Unknown internal error in CreateTask", Status = 500 };
            }
        }

        public async Task<DeleteTaskResponse> DeleteTask(string id)
        {
            int guid;

            if (string.IsNullOrEmpty(id) || !Int32.TryParse(id, out guid))
                return new DeleteTaskResponse { Message = "Invalid task id", Status = 400 };

            var task = await _tasksRepository.GetTaskById(guid);

            if (task == null)
            {
                return new DeleteTaskResponse { Message = "Task not found", Status = 404 };
            }

            if (await _tasksRepository.DeleteTask(task))
            {
                return new DeleteTaskResponse { Task = task, Message = "Success", Status = 200 };
            }
            else
            {
                return new DeleteTaskResponse { Message = "Unknown internal error in DeleteTask", Status = 500 };
            }
        }

        public async Task<EditTaskResponse> EditTask(EditTaskRequest request, string id)
        {
            int guid;

            if (string.IsNullOrEmpty(id) || !Int32.TryParse(id, out guid))
                return new EditTaskResponse { Message = "Invalid task id", Status = 400 };

            var task = await _tasksRepository.GetTaskById(guid);

            if (task == null)
            {
                return new EditTaskResponse { Message = "Task not found", Status = 404 };
            }

            try
            {
                request.Name ??= task.Name;
                request.Description ??= task.Description;
                request.Flag ??= task.Flag;
                request.File ??= task.File;
                request.Difficulty ??= task.Difficulty;
                request.Hints ??= task.Hints;
                request.Solution ??= task.Solution;
                request.SortOrder ??= task.SortOrder;

                task.Name = request.Name;
                task.Description = request.Description;
                task.Flag = request.Flag;
                task.File = request.File;
                task.Difficulty = request.Difficulty;
                task.Hints = request.Hints;
                task.Solution = request.Solution;
                task.SortOrder = (int)request.SortOrder;

                await _tasksRepository.SaveChangesAsync();

                return new EditTaskResponse { Task = task, Message = "Success", Status = 200 };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new EditTaskResponse { Message = "Unknown internal error in EditTask", Status = 500 };
            }
        }

        public async Task<GetTaskResponse> GetTask(string id)
        {
            int guid;

            if (string.IsNullOrEmpty(id) || !Int32.TryParse(id, out guid) || guid >= 2000000 || guid < 0)
                return new GetTaskResponse { Message = "Invalid task id", Status = 400 };

            var task = await _tasksRepository.GetTaskById(guid);

            if (task == null)
            {
                return new GetTaskResponse { Message = "Task not found", Status = 404 };
            }
            else
            {
                return new GetTaskResponse { Task = task, Message = "Success!", Status = 200 };
            }
        }

        public async Task<GetTasksListResponse> GetTasks(int page)
        {
            try
            {
                int pageSize = 16;

                if (page <= 0 || page > 4000)
                    return new GetTasksListResponse { Message = "Invalid task id", Status = 400 };

                var tasks = await _tasksRepository.GetTasks(page, pageSize);

                if (tasks == null || tasks.Count <= 0)
                    return new GetTasksListResponse { Message = "No tasks mathcing the request", Status = 404 };

                return new GetTasksListResponse { Tasks = tasks, Message = "Success", Status = 200 };

            }
            catch
            {
                Console.WriteLine("Internal error in GetTasks");
                return new GetTasksListResponse { Message = "Internal error", Status = 500 };
            }
        }

        public async Task<SolveTaskResponse> SolveTask(string id, string flag)
        {

            if (flag.Length > 500)
                return new SolveTaskResponse { Message = "Flag is too long", Status = 400 };


            int guid;

            if (string.IsNullOrEmpty(id) || !Int32.TryParse(id, out guid) || guid >= 2000000 || guid < 0)
                return new SolveTaskResponse { Message = "Invalid task id", Status = 400 };

            var task = await _tasksRepository.GetTaskById(guid);

            if (task == null)
            {
                return new SolveTaskResponse { Message = "Task not found", Status = 404 };
            }

            if (task.Flag == flag.Trim())
            {
                return new SolveTaskResponse { Message = "Solved!", Status = 200 };
            }
            else
            {
                return new SolveTaskResponse { Message = "Wrong flag", Status = 400 };
            }
        }

        public async Task<SolveTaskResponse> SolveTask(string id, string taskId, string flag)
        {
            if (flag.Length > 500)
                return new SolveTaskResponse { Message = "Flag is too long", Status = 400 };


            int guid;

            if (string.IsNullOrEmpty(taskId) || !Int32.TryParse(taskId, out guid) || guid >= 2000000 || guid < 0)
                return new SolveTaskResponse { Message = "Invalid task id", Status = 400 };

            var task = await _tasksRepository.GetTaskById(guid);

            if (task == null)
            {
                return new SolveTaskResponse { Message = "Task not found", Status = 404 };
            }

            if (task.Flag == flag.Trim())
            {
                await _serviceBus.PublishAsync(new TaskSolved(Guid.NewGuid(), Guid.Parse(id), guid, DateTime.UtcNow));
                return new SolveTaskResponse { Message = "Solved!", Status = 200 };
            }
            else
            {
                return new SolveTaskResponse { Message = "Wrong flag", Status = 400 };
            }
        }
    }
}
