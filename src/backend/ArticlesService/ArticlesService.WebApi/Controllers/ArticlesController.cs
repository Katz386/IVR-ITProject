using ArticlesService.Application.DTOs.Request;
using ArticlesService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArticlesService.WebApi.Controllers
{
    [ApiController]
    [Route("")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticlesService _articlesService;

        public ArticlesController(IArticlesService articlesService)
        {
            _articlesService = articlesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetArticles([FromQuery] int page)
        {
            var response = await _articlesService.GetArticles(page);

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticleById(string id)
        {
            var response = await _articlesService.GetArticleById(id);

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);
        }

        [HttpGet("preview/{id}")]
        public async Task<IActionResult> GetArticlePreviewById(string id)
        {
            var response = await _articlesService.GetArticlePreviewById(id);

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(string id)
        {
            var response = await _articlesService.DeleteArticle(id);

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditArticle(string id, [FromBody] EditArticleRequest request)
        {
            var response = await _articlesService.EditArticle(request, id);

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddArticle([FromBody] AddArticleRequest request)
        {
            var response = await _articlesService.AddArticle(request);

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);
        }
    }
}
