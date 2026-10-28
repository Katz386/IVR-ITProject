using FileService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileService.WebApi.Controllers
{
    [Route("")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFile(string id)
        {
            var response = await _fileService.GetFile(id);

            if (response.Content == null)
                return StatusCode(500, "Unknown serverv error. Failed to open file stream");

            return File(response.Content, "application/octet-stream", response.Name);
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return StatusCode(400, "File is required");

            var response = await _fileService.UploadFile(file);

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);

        }

        [HttpPost("tasks/upload")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadTaskFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return StatusCode(400, "File is required");

            var response = await _fileService.UploadTaskFile(file);

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);

        }

        [HttpGet("{id}/record")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetFileRecord(string id)
        {
            var response = await _fileService.GetFileRecord(id);

            if (response == null)
                return StatusCode(500, "Unknown serverv error");

            return StatusCode(response.Status, response);

        }
    }
}
