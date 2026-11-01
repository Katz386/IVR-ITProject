using Microsoft.EntityFrameworkCore;
using PlatformService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformService.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<PlatformStatistics> Statistics => Set<PlatformStatistics>();

        public AppDbContext(DbContextOptions options) : base(options) { }
    }
}
