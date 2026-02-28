using System;
using System.Collections.Generic;
using System.Text;
using ExpenseClaimSystem.Application.DTOs;
using System.Threading.Tasks;
using System.Linq;

namespace ExpenseClaimSystem.Application.Interfaces
{
    public interface IAuthService
    {
        // Returns tuple with success flag and possible error messages
        Task<(bool Succeeded, string[] Errors)> RegisterAsync(RegisterRequest request);
        // Returns a JWT access token on success, or null on failure
        Task<string?> LoginAsync(LoginRequest request);
        Task LogoutAsync();
    }
}
