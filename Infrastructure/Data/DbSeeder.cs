using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        string[] roles = { UserRole.Admin, UserRole.Instructor, UserRole.Student };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<int>(role));
        }

        if (await userManager.FindByEmailAsync("admin@courses.com") is null)
        {
            var admin = new ApplicationUser
            {
                FullName = "Administrator",
                Email = "admin@courses.com",
                UserName = "admin@courses.com",
                CreatedAt = DateTime.UtcNow
            };

            await userManager.CreateAsync(admin, "Admin@123!");
            await userManager.AddToRoleAsync(admin, UserRole.Admin);
        }
    }
}