using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Request;
using UserService.Application.Interfaces;

namespace UserService.WebApi.Controllers
{
    [Route("")]
    [ApiController]
    public class UserServiceController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserServiceController (IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfileById(string id)
        {
            var response = await _userService.GetProfile(id);

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            if (response.Profile.IsPrivate == true)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    string? uid = User.FindFirst("uid")?.Value;

                    if (uid == null)
                        return StatusCode(401, new { Message = "Missing uid field in token" });

                    if (uid == id)
                    {
                        return StatusCode(response.Status, response);
                    }
                    return StatusCode(403, new { Message = "Private profile" });
                }
                else
                {
                    return StatusCode(403, new { Message = "Private profile" });
                }
            }

            return StatusCode(response.Status, response);
        }

        [HttpGet("profile/me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            string? uid = User.FindFirst("uid")?.Value;

            if (uid == null)
                return StatusCode(401, new { Message = "Missing uid field in token" });

            var response = await _userService.GetProfile(uid);

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);
        }

        [Authorize]
        [HttpPost("profile/articles")]
        public async Task<IActionResult> ReadArticle([FromBody] ReadArticleRequest request)
        {
            string? id = User.FindFirst("uid")?.Value;

            if (id == null)
                return StatusCode(401, new { Message = "Missing uid field in token" });

            var response = await _userService.ReadArticle(id, request.Id);

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);
        }

        [Authorize]
        [HttpPost("profile")]
        public async Task<IActionResult> EditProfile([FromBody] EditProfileRequest request)
        {
            string? id = User.FindFirst("uid")?.Value;

            if (id == null)
                return StatusCode(401, new { Message = "Missing uid field in token" });

            var response = await _userService.EditProfile(id, request);

            if (response == null)
                return StatusCode(500, new { Message = "Unhandled internal error", Status = 500 });

            return StatusCode(response.Status, response);
        }
    }
}
