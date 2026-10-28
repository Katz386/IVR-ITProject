using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.Interfaces;
using UserService.Domain.Models;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository (AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateProfile(Profile profile)
        {
            try
            {
                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\"{ex.Message}\" in CreateProfile");
                return false;
            }
        }

        public async Task<Profile?> GetProfileByAccountId(Guid id)
        {
            try
            {
                return await _context.Profiles.Where(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\"{ex.Message}\" in GetProfileByAccountId");
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
                Console.WriteLine($"\"{ex.Message}\" in SaveChangesAsync");
                return false;
            }
        }
    }
}
