using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TasksService.Domain.Models
{
    public class WebTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string Flag {  get; set; }
        public string File { get; set; }
        public string Difficulty { get; set; }
        public string[] Hints { get; set; }
        public string Solution { get; set; }
        public DateTime Created { get; set; }
        public int SortOrder {  get; set; }
    }
}
