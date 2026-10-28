using AuthService.Application.DTOs.Request;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.WebApi.Controllers
{
    [ApiController]
    public class AuthContoller : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthContoller(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest? request)
        {
            if (request == null)
                return BadRequest(new { Message = "Bad request" });

            try
            {
                var response = await _authService.SignInAsync(request);
                return StatusCode(response.StatusCode, new { message = response.Message, token = response.Token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Unknown exception" });
            }
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest? request)
        {
            if (request == null)
                return BadRequest(new { Message = "Bad request" });

            try
            {
                var response = await _authService.SignUpAsync(request);

                return StatusCode(response.StatusCode, new { message = response.Message, token = response.Token });
            }
            catch (FormatException ex)
            {
                return BadRequest(new { Message = "Format exception" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Unknown exception" });
            }
        }
    }
}
