using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseClaimSystem.Application.DTOs;
using ExpenseClaimSystem.Application.Interfaces;
using ExpenseClaimSystem.Infrastructure.Services;
using ExpenseClaimSystem.Domain.Entities;
using ExpenseClaimSystem.Domain.Enums;
using Xunit;

namespace ExpenseClaimSystem.UnitTests
{
    public class ExpenseClaimServiceTests
    {
        private class FakeRepository : IExpenseClaimRepository
        {
            public ExpenseClaim? LastAddedClaim { get; private set; }
            public AttachmentDetails? LastAddedAttachment { get; private set; }
            public int CountReturn { get; set; } = 0;
            public bool SaveCalled { get; private set; } = false;

            public Task AddAsync(ExpenseClaim claim)
            {
                LastAddedClaim = claim;
                return Task.CompletedTask;
            }

            public Task AddAttachmentAsync(Guid claimId, string fileName, string filePath, string type)
            {
                LastAddedAttachment = new AttachmentDetails
                {
                    ExpenseClaimId = claimId,
                    FileName = fileName,
                    FilePath = filePath,
                    AttachmentType = type
                };
                return Task.CompletedTask;
            }

            public Task AddAttachmentAsync(AttachmentDetails attachment)
            {
                LastAddedAttachment = attachment;
                return Task.CompletedTask;
            }

            public Task<int> CountAsync()
            {
                return Task.FromResult(CountReturn);
            }

            public Task<List<ExpenseClaim>> GetAllAsync()
            {
                return Task.FromResult(new List<ExpenseClaim>());
            }

            public Task<List<ExpenseClaim>> GetFilteredAsync(ExpenseClaimFilterDto filter)
            {
                return Task.FromResult(new List<ExpenseClaim>());
            }

            public Task SaveChangesAsync()
            {
                SaveCalled = true;
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task CreateAsync_Generates_RequestNumber_BasedOnCount()
        {
            var repo = new FakeRepository { CountReturn = 0 };
            var svc = new ExpenseClaimService(repo);

            var dto = new CreateExpenseClaimDto();
            var id = await svc.CreateAsync(dto, "user1");

            Assert.NotNull(repo.LastAddedClaim);
            Assert.Equal("ECR-00001", repo.LastAddedClaim.RequestNumber);
        }

        [Fact]
        public async Task CreateAsync_Calculates_Subtotal_Gst_And_Total()
        {
            var repo = new FakeRepository { CountReturn = 5 };
            var svc = new ExpenseClaimService(repo);

            var dto = new CreateExpenseClaimDto();
            dto.Items.Add(new ExpenseItemDto { Amount = 100m, IsGstApplicable = true });
            dto.Items.Add(new ExpenseItemDto { Amount = 50m, IsGstApplicable = false });

            var id = await svc.CreateAsync(dto, "user2");

            Assert.NotNull(repo.LastAddedClaim);
            Assert.Equal(150m, repo.LastAddedClaim.SubTotal);
            Assert.Equal(Math.Round(100m * 0.08m, 2), repo.LastAddedClaim.GstAmount);
            Assert.Equal(repo.LastAddedClaim.SubTotal + repo.LastAddedClaim.GstAmount, repo.LastAddedClaim.TotalAmount);
        }

        [Fact]
        public async Task CreateAsync_Adds_Attachments_To_Claim()
        {
            var repo = new FakeRepository();
            var svc = new ExpenseClaimService(repo);

            var dto = new CreateExpenseClaimDto();
            dto.Attachments.Add(new ExpenseAttachmentDto { FileName = "a.txt", FilePath = "/tmp/a.txt", AttachmentType = "Receipt" });

            var id = await svc.CreateAsync(dto, "user3");

            // The service adds attachments to the claim instance after saving; repository should hold reference
            Assert.NotNull(repo.LastAddedClaim);
            Assert.Single(repo.LastAddedClaim.Attachments);
            var att = repo.LastAddedClaim.Attachments.First();
            Assert.Equal("a.txt", att.FileName);
            Assert.Equal("/tmp/a.txt", att.FilePath);
            Assert.Equal("Receipt", att.AttachmentType);
        }

        [Fact]
        public async Task AddAttachmentAsync_Calls_Repository_With_Correct_Values()
        {
            var repo = new FakeRepository();
            var svc = new ExpenseClaimService(repo);

            var claimId = Guid.NewGuid();
            await svc.AddAttachmentAsync(claimId, "b.pdf", "/tmp/b.pdf", "Invoice");

            Assert.True(repo.SaveCalled);
            Assert.NotNull(repo.LastAddedAttachment);
            Assert.Equal(claimId, repo.LastAddedAttachment.ExpenseClaimId);
            Assert.Equal("b.pdf", repo.LastAddedAttachment.FileName);
            Assert.Equal("/tmp/b.pdf", repo.LastAddedAttachment.FilePath);
            Assert.Equal("Invoice", repo.LastAddedAttachment.AttachmentType);
        }

        [Fact]
        public async Task CreateAsync_Sets_EmployeeId_And_Currency_And_PaymentMethod()
        {
            var repo = new FakeRepository();
            var svc = new ExpenseClaimService(repo);

            var dto = new CreateExpenseClaimDto { Currency = "USD", PaymentMethod = PaymentMethod.Card };

            var id = await svc.CreateAsync(dto, "employee-42");

            Assert.NotNull(repo.LastAddedClaim);
            Assert.Equal("employee-42", repo.LastAddedClaim.EmployeeId);
            Assert.Equal("USD", repo.LastAddedClaim.Currency);
            Assert.Equal(PaymentMethod.Card, repo.LastAddedClaim.PaymentMethod);
        }
    }
}
