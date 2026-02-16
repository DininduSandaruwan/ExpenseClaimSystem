using System;
using System.Collections.Generic;
using System.Text;
using ExpenseClaimSystem.Application.DTOs;

namespace ExpenseClaimSystem.Application.Services
{
    public interface IExpenseClaimService
    {
        Task<Guid> CreateAsync(CreateExpenseClaimDto dto, string userId);
    }
}
