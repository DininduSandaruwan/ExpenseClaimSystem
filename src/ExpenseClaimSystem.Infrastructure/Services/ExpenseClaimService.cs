using ExpenseClaimSystem.Application.DTOs;
using ExpenseClaimSystem.Application.Interfaces;
using ExpenseClaimSystem.Application.Services;
using ExpenseClaimSystem.Domain.Entities;
using ExpenseClaimSystem.Domain.Enums;

namespace ExpenseClaimSystem.Infrastructure.Services
{
    public class ExpenseClaimService : IExpenseClaimService
    {
        private readonly IExpenseClaimRepository _repository;

        public ExpenseClaimService(IExpenseClaimRepository repository)
        {
            _repository = repository;
        }
        public Task<List<ExpenseClaim>> GetFilteredAsync(ExpenseClaimFilterDto filter)
        {
            return _repository.GetFilteredAsync(filter);
        }

        public async Task<Guid> CreateAsync(CreateExpenseClaimDto dto, string userId)
        {
            var count = await _repository.CountAsync();
            var requestNumber = $"ECR-{(count + 1).ToString("D5")}";

            var claim = new ExpenseClaim
            {
                RequestNumber = requestNumber,
                EmployeeId = userId,
                Department = dto.Department,
                Currency = dto.Currency,
                PaymentMethod = dto.PaymentMethod
            };

            decimal subtotal = 0;
            decimal gst = 0;

            foreach (var item in dto.Items)
            {
                claim.Items.Add(new ExpenseItem
                {
                    Category = item.Category,
                    Description = item.Description,
                    Amount = item.Amount,
                    IsGstApplicable = item.IsGstApplicable
                });

                subtotal += item.Amount;

                if (item.IsGstApplicable)
                    gst += item.Amount * 0.08m;
            }

            claim.SubTotal = subtotal;
            claim.GstAmount = gst;
            claim.TotalAmount = subtotal + gst;

            await _repository.AddAsync(claim);
            await _repository.SaveChangesAsync();

            foreach (var attachment in dto.Attachments)
            {
                claim.Attachments.Add(new AttachmentDetails
                {
                    FileName = attachment.FileName,
                    FilePath = attachment.FilePath,
                    AttachmentType = attachment.AttachmentType
                });
            }

            return claim.Id;
        }
        public async Task AddAttachmentAsync(Guid claimId, string fileName, string filePath, string type)
        {
            var attachment = new AttachmentDetails
            {
                ExpenseClaimId = claimId,
                FileName = fileName,
                FilePath = filePath,
                AttachmentType = type
            };

            await _repository.AddAttachmentAsync(attachment);
            await _repository.SaveChangesAsync();
        }
    }
}
