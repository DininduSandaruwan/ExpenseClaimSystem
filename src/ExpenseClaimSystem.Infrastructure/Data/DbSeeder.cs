using ExpenseClaimSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ExpenseClaimSystem.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager,
                                           RoleManager<IdentityRole> roleManager)
        {
            // 1. Define roles
            string[] roles = new[] { "Employee", "Manager", "Finance" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Create an admin user (optional)
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123"); // Password
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(adminUser, roles); // Add all roles
                }
            }

            // 3. Optionally, create more default users
            var employeeEmail = "employee@example.com";
            if (await userManager.FindByEmailAsync(employeeEmail) == null)
            {
                var employee = new ApplicationUser
                {
                    UserName = employeeEmail,
                    Email = employeeEmail,
                    FullName = "Default Employee",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(employee, "Employee@123");
                await userManager.AddToRoleAsync(employee, "Employee");
            }
        }
    }
}
