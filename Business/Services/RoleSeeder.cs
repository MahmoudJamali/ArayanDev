using DataAccess.Concrete.Contexts;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

public static class RoleSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Roles.AnyAsync())
            return;

        var roles = new List<Role>
        {
            new Role { Name = "Admin", Description = "مدیر سیستم" },
            new Role { Name = "User", Description = "کاربر عادی" },
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }
}
