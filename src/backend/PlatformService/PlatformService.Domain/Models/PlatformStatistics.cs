using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformService.Domain.Models
{
    public class PlatformStatistics
    {
        public int Id { get; set; }
        public int EasyTasksCount { get; set; }
        public int MediumTasksCount { get; set; }
        public int HardTasksCount { get; set; }
        public int ArticlesCount { get; set; }
        public int UsersCount { get; set; }
    }
}
