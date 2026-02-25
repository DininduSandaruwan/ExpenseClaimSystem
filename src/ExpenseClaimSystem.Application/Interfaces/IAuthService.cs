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
        Task<bool> LoginAsync(LoginRequest request);
        Task LogoutAsync();
    }
}
