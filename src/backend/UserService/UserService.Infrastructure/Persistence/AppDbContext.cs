using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Models;

namespace UserService.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Profile> Profiles => Set<Profile>();

        public AppDbContext(DbContextOptions options) : base (options) { }
    }
}
