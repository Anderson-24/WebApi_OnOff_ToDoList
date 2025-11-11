using Microsoft.AspNetCore.Mvc;
using WebApi_OnOff_ToDoList.Application.DTOs;
using WebApi_OnOff_ToDoList.Application.Interfaces;

namespace WebApi_OnOff_ToDoList.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            if (response == null)
                return Unauthorized(new { message = "Invalid credentials." });

            return Ok(response);
        }
    }
}
