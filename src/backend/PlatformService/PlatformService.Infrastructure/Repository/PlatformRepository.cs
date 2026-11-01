using Microsoft.EntityFrameworkCore;
using PlatformService.Application.Interfaces;
using PlatformService.Domain.Models;
using PlatformService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformService.Infrastructure.Repository
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly AppDbContext _context;

        public PlatformRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PlatformStatistics?> GetStatistics()
        {
            try
            {
                return await _context.Statistics.FirstOrDefaultAsync(x => x.Id == 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} in GetStatistics");
                return null;
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} in SaveChangesAsync");
                return false;
            }
        }
    }
}
