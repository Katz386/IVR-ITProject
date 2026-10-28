using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Models;

namespace UserService.Application.Interfaces
{
    public interface IUserRepository
    {
        public Task<bool> CreateProfile(Profile profile);
        public Task<Profile?> GetProfileByAccountId(Guid id);
        public Task<bool> SaveChangesAsync();
    }
}
