using PlatformService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PlatformService.Application.DTOs.Response
{
    public class GetStatisticsResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlatformStatistics? Statistics { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
    }
}
