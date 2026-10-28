using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TasksService.Application.DTOs.Request;
using TasksService.Application.DTOs.Response;
using TasksService.Application.Interfaces;

namespace TasksService.WebApi.Controllers
{
    [ApiController]
    [Route("")]
    public class TasksController : ControllerBase
    {
        private readonly ITasksService _taskService;

        public TasksController (ITasksService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(string id)
        {
            var response = await _taskService.GetTask(id);

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] int page)
        {
            var response = await _taskService.GetTasks(page);

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            var response = await _taskService.DeleteTask(id);

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditTask(string id, [FromBody] EditTaskRequest request)
        {
            var response = await _taskService.EditTask(request, id);

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddTask([FromBody] CreateTaskRequest request)
        {
            var response = await _taskService.CreateTask(request);

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);
        }

        [HttpPost("{id}/solve")]
        [AllowAnonymous]
        public async Task<IActionResult> SolveTask(string id, [FromBody] SolveTaskRequest request)
        {
            SolveTaskResponse response;

            if (User.Identity?.IsAuthenticated == true)
            {
                string? uid = User.FindFirst("uid")?.Value;

                if (uid == null)
                    return StatusCode(401, new { Message = "Missing uid field in token" });

                response = await _taskService.SolveTask(uid, id, request.Flag);
            }
            else
            {
                response = await _taskService.SolveTask(id, request.Flag);
            }

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);
        }
    }
}
