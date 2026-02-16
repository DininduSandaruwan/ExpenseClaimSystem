using ExpenseClaimSystem.Application.Interfaces;
using ExpenseClaimSystem.Domain.Entities;
using ExpenseClaimSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseClaimSystem.Infrastructure.Repositories
{
    public class ExpenseClaimRepository : IExpenseClaimRepository
    {
        private readonly ApplicationDbContext _context;

        public ExpenseClaimRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ExpenseClaim claim)
        {
            await _context.ExpenseClaims.AddAsync(claim);
        }

        public async Task AddAttachmentAsync(Domain.Entities.Attachment attachment)
        {
            await _context.Attachments.AddAsync(attachment);
        }

        public Task AddAttachmentAsync(Guid claimId, string fileName, string filePath, string type)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CountAsync()
        {
            return await _context.ExpenseClaims.CountAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
