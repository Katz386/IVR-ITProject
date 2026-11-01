using PlatformService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformService.Application.Interfaces
{
    public interface IPlatformRepository
    {
        public Task<bool> SaveChangesAsync();
        public Task<PlatformStatistics?> GetStatistics();
    }
}
