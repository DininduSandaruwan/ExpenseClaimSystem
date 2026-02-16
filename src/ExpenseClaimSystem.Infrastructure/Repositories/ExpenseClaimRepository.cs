using ExpenseClaimSystem.Application.DTOs;
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
        public Task<List<ExpenseClaim>> GetAllAsync()
        {
            return _context.ExpenseClaims
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
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
        public async Task<List<ExpenseClaim>> GetFilteredAsync(ExpenseClaimFilterDto filter)
        {
            var query = _context.ExpenseClaims.AsQueryable();

            if (filter.Status.HasValue)
                query = query.Where(x => x.Status == filter.Status.Value);

            if (!string.IsNullOrWhiteSpace(filter.Department))
                query = query.Where(x => x.Department == filter.Department);

            if (filter.FromDate.HasValue)
                query = query.Where(x => x.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(x => x.CreatedAt <= filter.ToDate.Value);

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

    }
}
