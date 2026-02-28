using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using ExpenseClaimSystem.Infrastructure.Data;
using ExpenseClaimSystem.Infrastructure.Repositories;
using ExpenseClaimSystem.Infrastructure.Services;
using ExpenseClaimSystem.Application.DTOs;
using ExpenseClaimSystem.Domain.Entities;
using ExpenseClaimSystem.Domain.Enums;

namespace ExpenseClaimSystem.IntegrationTests
{
    public class ExpenseClaimServiceIntegrationTests
    {
        private static ApplicationDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_Persists_Claim_To_Database()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var repo = new ExpenseClaimRepository(context);
            var svc = new ExpenseClaimService(repo);

            var dto = new CreateExpenseClaimDto();
            dto.Items.Add(new ExpenseItemDto { Amount = 200m, IsGstApplicable = true });
            dto.Attachments.Add(new ExpenseAttachmentDto { FileName = "r.pdf", FilePath = "/tmp/r.pdf", AttachmentType = "Receipt" });

            var id = await svc.CreateAsync(dto, "int-user");

            var saved = await context.ExpenseClaims.Include(x => x.Items).Include(x => x.Attachments).FirstOrDefaultAsync(x => x.Id == id);
            Assert.NotNull(saved);
            Assert.Equal("int-user", saved.EmployeeId);
            Assert.Single(saved.Items);
            Assert.Single(saved.Attachments);
            Assert.Equal(200m, saved.SubTotal);
            Assert.Equal(Math.Round(200m * 0.08m, 2), saved.GstAmount);
        }

        [Fact]
        public async Task GetFilteredAsync_Filters_By_Department_And_Status()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            // seed
            context.ExpenseClaims.Add(new ExpenseClaim { Department = "HR", Status = ClaimStatus.Pending, RequestNumber = "ECR-00001" });
            context.ExpenseClaims.Add(new ExpenseClaim { Department = "Finance", Status = ClaimStatus.Approved, RequestNumber = "ECR-00002" });
            await context.SaveChangesAsync();

            var repo = new ExpenseClaimRepository(context);

            var list = await repo.GetFilteredAsync(new Application.DTOs.ExpenseClaimFilterDto { Department = "HR" });
            Assert.Single(list);
            Assert.Equal("HR", list.First().Department);

            list = await repo.GetFilteredAsync(new Application.DTOs.ExpenseClaimFilterDto { Status = ClaimStatus.Approved });
            Assert.Single(list);
            Assert.Equal(ClaimStatus.Approved, list.First().Status);
        }

        [Fact]
        public async Task CountAsync_Returns_Number_Of_Claims()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            context.ExpenseClaims.Add(new ExpenseClaim { Department = "A" });
            context.ExpenseClaims.Add(new ExpenseClaim { Department = "B" });
            await context.SaveChangesAsync();

            var repo = new ExpenseClaimRepository(context);
            var count = await repo.CountAsync();
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task AddAttachmentAsync_Persists_Attachment_Record()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var repo = new ExpenseClaimRepository(context);
            var svc = new ExpenseClaimService(repo);

            var claim = new ExpenseClaim();
            context.ExpenseClaims.Add(claim);
            await context.SaveChangesAsync();

            await svc.AddAttachmentAsync(claim.Id, "doc.pdf", "/tmp/doc.pdf", "Invoice");

            var att = await context.Attachments.FirstOrDefaultAsync(a => a.ExpenseClaimId == claim.Id);
            Assert.NotNull(att);
            Assert.Equal("doc.pdf", att.FileName);
            Assert.Equal("/tmp/doc.pdf", att.FilePath);
            Assert.Equal("Invoice", att.AttachmentType);
        }

        [Fact]
        public async Task CreateAsync_Increments_RequestNumber()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var repo = new ExpenseClaimRepository(context);
            var svc = new ExpenseClaimService(repo);

            var dto1 = new CreateExpenseClaimDto();
            var id1 = await svc.CreateAsync(dto1, "u1");

            var dto2 = new CreateExpenseClaimDto();
            var id2 = await svc.CreateAsync(dto2, "u2");

            var c1 = await context.ExpenseClaims.FindAsync(id1);
            var c2 = await context.ExpenseClaims.FindAsync(id2);

            Assert.NotNull(c1);
            Assert.NotNull(c2);
            Assert.NotEqual(c1.RequestNumber, c2.RequestNumber);
        }
    }
}
