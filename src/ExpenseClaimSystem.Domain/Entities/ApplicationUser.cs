using Microsoft.AspNetCore.Identity;

namespace ExpenseClaimSystem.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}