using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EmployeeManagement.Infrastructure
{
    public static class SeedData
    {
        public static async Task InitializeAsync(AppDbContext context, UserManager<Employee> userManager)
        {
            // Apply pending migrations
            await context.Database.MigrateAsync();

            var existing = await userManager.FindByEmailAsync("admin@example.com");

            if (existing != null) return;

            var adminEmployee = new Employee(
                firstName: "Super",
                lastName: "Admin",
                email: "admin@example.com",
                department: "HR",
                passwordHash: string.Empty,
                role: "Admin"
            );

            var create = await userManager.CreateAsync(adminEmployee, "Admin123!");
            if (!create.Succeeded) return;
        }
    }
}
