using Microsoft.AspNetCore.Mvc;
using PlatformService.Application.Interfaces;

namespace PlatformService.WebApi.Controllers
{
    [Route("")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformService _platformService;

        public PlatformController(IPlatformService platformService)
        {
            _platformService = platformService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var response = await _platformService.GetStatistics();

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);
        }
    }
}
