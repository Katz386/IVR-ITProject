using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Domain.Models;
using PlatformService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformService.Infrastructure
{
    public static class StatisticsSeeder
    {
        public static async Task SeedStatistics(IServiceProvider services, IConfiguration configuration)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var statistics = await db.Statistics.FirstOrDefaultAsync(u => u.Id == 1);

            if (statistics != null)
            {
                return;
            }

            var s = new PlatformStatistics
            {
                Id = 1,
                EasyTasksCount = 0,
                MediumTasksCount = 0,
                HardTasksCount = 0,
                UsersCount = 0
            };

            db.Statistics.Add(s);
            await db.SaveChangesAsync();
        }
    }
}
