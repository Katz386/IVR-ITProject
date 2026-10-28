using AuthService.Domain.Models;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services, IConfiguration configuration)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var adminUsername = configuration["ADMIN_USERNAME"];
            var adminPassword = configuration["ADMIN_PASSWORD"];

            if (string.IsNullOrWhiteSpace(adminUsername) ||
                string.IsNullOrWhiteSpace(adminPassword))
            {
                Console.WriteLine("[X] AdminSeeder: No ADMIN_USERNAME or ADMIN_PASSWORD provided");
                return;
            }

            var admin = await db.Users.FirstOrDefaultAsync(u => u.Username == adminUsername);

            if (admin != null)
            {
                Console.WriteLine("[i] AdminSeeder: Admin user already exists");
                return;
            }

            var newAdmin = new User
            {
                UID = Guid.NewGuid(),
                Username = adminUsername,
                Role = "Admin"
            };

            newAdmin.PasswordHash = User.HashPassword(adminPassword);

            db.Users.Add(newAdmin);
            await db.SaveChangesAsync();

            Console.WriteLine("[+] AdminSeeder: Admin user successfully created");
        }
    }
}
