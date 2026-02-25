using ExpenseClaimSystem.Application.DTOs;
using ExpenseClaimSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseClaimSystem.BlazorServer.Controllers
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, errors) = await _authService.RegisterAsync(request);

            if (!succeeded)
                return BadRequest(new { Errors = errors });

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _authService.LoginAsync(request);
            if (!success)
                return Unauthorized();

            return Ok();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return Ok();
        }
    }
}
