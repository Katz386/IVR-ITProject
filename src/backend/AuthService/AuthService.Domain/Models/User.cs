using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Models
{
    public class User
    {
        [Key]
        public Guid UID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public string Role { get; set; }

        public bool VerifyPassword(string password)
        {
            var hash = HashPassword(password);
            return PasswordHash == hash;
        }

        public static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
