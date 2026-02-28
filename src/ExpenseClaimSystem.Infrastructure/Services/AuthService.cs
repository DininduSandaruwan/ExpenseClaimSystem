using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using ExpenseClaimSystem.Application.DTOs;
using ExpenseClaimSystem.Application.Interfaces;
using ExpenseClaimSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ExpenseClaimSystem.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        // Constructor used by DI - IConfiguration is required for JWT settings
        // Removed the older two-parameter constructor to ensure IConfiguration is injected.

        public async Task<(bool Succeeded, string[] Errors)> RegisterAsync(RegisterRequest request)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors != null ? result.Errors.Select(e => e.Description).ToArray() : new string[] { "Unknown error" };
                return (false, errors);
            }

            await _userManager.AddToRoleAsync(user, "Employee");

            return (true, Array.Empty<string>());
        }

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<string?> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    Console.WriteLine("User not found!");
                    return null;
                }

                // Validate password
                var valid = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!valid)
                {
                    Console.WriteLine("Invalid password");
                    return null;
                }

                // Create claims
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                    new Claim("fullName", user.FullName ?? string.Empty),
                    // Also include the standard Name claim so various clients/readers can pick it up
                    new Claim(ClaimTypes.Name, user.FullName ?? user.UserName ?? string.Empty)
                };

                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                // Read JWT settings from configuration
                var jwtKey = _configuration["Jwt:Key"] ?? "ReplaceWithStrongKeyForProduction";
                var jwtIssuer = _configuration["Jwt:Issuer"] ?? "ExpenseClaimSystem";
                var jwtAudience = _configuration["Jwt:Audience"] ?? "ExpenseClaimSystemClients";
                var expiresMinutes = 60;
                if (int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var em))
                    expiresMinutes = em;

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                    signingCredentials: creds
                );

                var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
                return tokenStr;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error: {ex.ToString()}");
                return null;
            }
        }


        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
