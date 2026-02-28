using ExpenseClaimSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseClaimSystem.BlazorServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            // Try to get user id from claims
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ApplicationUser user = null;

            if (!string.IsNullOrEmpty(userId))
            {
                user = await _userManager.FindByIdAsync(userId);
            }

            if (user == null && User.Identity?.IsAuthenticated == true)
            {
                var name = User.Identity.Name;
                if (!string.IsNullOrEmpty(name))
                {
                    user = await _userManager.FindByNameAsync(name);
                }
            }

            if (user == null)
                return Unauthorized();

            return Ok(new { FullName = user.FullName ?? user.UserName, Email = user.Email, UserName = user.UserName });
        }
    }
}
