using System;
using System.Collections.Generic;
using System.Text;
using ExpenseClaimSystem.Application.DTOs;
using ExpenseClaimSystem.Domain.Entities;

namespace ExpenseClaimSystem.Application.Services
{
    public interface IExpenseClaimService
    {
        Task<Guid> CreateAsync(CreateExpenseClaimDto dto, string userId); 
        Task<List<ExpenseClaim>> GetFilteredAsync(ExpenseClaimFilterDto filter);

    }
}
