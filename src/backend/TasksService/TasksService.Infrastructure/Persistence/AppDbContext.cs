using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksService.Domain.Models;

namespace TasksService.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<WebTask> Tasks => Set<WebTask>();
        public AppDbContext(DbContextOptions options) : base(options) { }
    }
}
