
using ExpenseClaimSystem.Application.DTOs;
using ExpenseClaimSystem.Application.Interfaces;
using ExpenseClaimSystem.BlazorClient;
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

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return false;

            await _userManager.AddToRoleAsync(user, "Employee");

            return true;
        }

        //public async Task<bool> LoginAsync(LoginRequest request)
        //{
        //    var result = await _signInManager.PasswordSignInAsync(
        //        request.Email,
        //        request.Password,
        //        false,
        //        false);

        //    return result.Succeeded;
        //}

        public async Task<bool> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                Console.WriteLine("User not found!");
                return false;
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                request.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            Console.WriteLine($"Login result: {result.Succeeded}");
            return result.Succeeded;
        }


        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
