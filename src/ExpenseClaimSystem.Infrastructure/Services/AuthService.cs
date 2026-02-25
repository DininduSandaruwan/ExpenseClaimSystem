using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

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

        public async Task<bool> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    Console.WriteLine("User not found!");
                    return false;
                }

                Console.WriteLine($"Email: {request.Email}");
                Console.WriteLine($"Password: {request.Password}");

                var result = await _signInManager.PasswordSignInAsync(
                    user.Email,
                    request.Password,
                    isPersistent: false,
                    lockoutOnFailure: false);

                Console.WriteLine($"Login result: {result.Succeeded}");
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error: {ex.ToString()}");
                return false;
            }
        }


        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
