using ExpenseClaimSystem.Application.DTOs;
using ExpenseClaimSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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

            var token = await _authService.LoginAsync(request);
            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized();

            try
            {
                // Try parse token to get expiry
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var expires = jwt.ValidTo; // UTC

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = false, // allow client-side JS to read the token (needed for dashboard display)
                    Expires = expires,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                };

                Response.Cookies.Append("authToken", token, cookieOptions);
            }
            catch
            {
                // If parsing or cookie set fails, ignore and still return token in body
            }

            return Ok(new { token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            try
            {
                Response.Cookies.Delete("authToken");
            }
            catch
            {
                // ignore
            }
            return Ok();
        }

        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var user = HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return Unauthorized();

            var fullName = user.FindFirst("fullName")?.Value
                           ?? user.FindFirst(ClaimTypes.Name)?.Value
                           ?? user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value
                           ?? string.Empty;

            var staffId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? user.FindFirst("sub")?.Value
                          ?? user.FindFirst("staffId")?.Value
                          ?? string.Empty;

            var department = user.FindFirst("department")?.Value
                             ?? user.FindFirst("Department")?.Value
                             ?? string.Empty;

            return Ok(new { fullName, staffId, department });
        }
    }
}
