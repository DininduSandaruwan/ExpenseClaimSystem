using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using ExpenseClaimSystem.Domain.Entities;

namespace ExpenseClaimSystem.Application.Interfaces
{
    public interface IExpenseClaimRepository
    {
        Task AddAsync(ExpenseClaim claim);
        Task AddAttachmentAsync(Guid claimId, string fileName, string filePath, string type);
        Task AddAttachmentAsync(Domain.Entities.Attachment attachment);
        Task<int> CountAsync();
        Task SaveChangesAsync();
    }
}
