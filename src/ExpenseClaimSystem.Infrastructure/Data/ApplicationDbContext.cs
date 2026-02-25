using ExpenseClaimSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseClaimSystem.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ExpenseClaim> ExpenseClaims => Set<ExpenseClaim>();
        public DbSet<ExpenseItem> ExpenseItems => Set<ExpenseItem>();
        public DbSet<AttachmentDetails> Attachments => Set<AttachmentDetails>();


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ExpenseClaim>()
                .HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(x => x.ExpenseClaimId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ExpenseClaim>()
                .HasMany(x => x.Attachments)
                .WithOne()
                .HasForeignKey(x => x.ExpenseClaimId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
