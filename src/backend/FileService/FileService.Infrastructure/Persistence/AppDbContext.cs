using FileService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<FileRecord> Files => Set<FileRecord>();
        public AppDbContext(DbContextOptions options) : base (options)
        {

        }
    }
}
