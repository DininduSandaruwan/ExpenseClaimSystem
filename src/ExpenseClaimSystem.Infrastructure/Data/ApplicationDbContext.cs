using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection.Emit;
using System.Text;
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
        //public DbSet<ExpenseClaim> ExpenseClaims => Set<ExpenseClaim>();
        //public DbSet<ExpenseItem> ExpenseItems => Set<ExpenseItem>();
        //public DbSet<Attachment> Attachments => Set<Attachment>();


        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    builder.Entity<ExpenseClaim>()
        //        .HasMany(x => x.Items)
        //        .WithOne()
        //        .HasForeignKey(x => x.ExpenseClaimId);

        //    builder.Entity<ExpenseClaim>()
        //        .HasMany(x => x.Attachments)
        //        .WithOne()
        //        .HasForeignKey(x => x.ExpenseClaimId);
        //}
    }
}
