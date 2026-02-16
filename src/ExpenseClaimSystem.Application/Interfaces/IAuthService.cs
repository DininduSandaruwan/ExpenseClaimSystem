using System;
using System.Collections.Generic;
using System.Text;
using ExpenseClaimSystem.Application.DTOs;

namespace ExpenseClaimSystem.Application.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<bool> LoginAsync(LoginRequest request);
        Task LogoutAsync();
    }
}
