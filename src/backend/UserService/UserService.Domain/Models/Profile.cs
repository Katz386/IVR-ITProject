using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UserService.Domain.Models
{
    public class Profile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string TasksSolved { get; set; }
        public string ArticlesRead { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime Created {  get; set; }
    }
}
