using ArticlesService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesService.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Article> Articles => Set<Article>();
        public AppDbContext(DbContextOptions options) : base(options) { }
    }
}
