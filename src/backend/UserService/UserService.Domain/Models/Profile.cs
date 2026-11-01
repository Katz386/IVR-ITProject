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
        public int ArticlesReadCount { get; set; }
        public int EasyTasksSolvedCount { get; set; }
        public int MediumTasksSolvedCount { get; set; }
        public int HardTasksSolvedCount { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime LastActivity {  get; set; }
        public DateTime? LastTaskSolvedDate { get; set; }
        public int LastTaskSolvedId { get; set; }
        public DateTime Created {  get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string AdminComment { get; set; }
    }
}
