using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TasksService.Domain.Models;

namespace TasksService.Application.DTOs.Response
{
    public class BaseTasksResponse
    {
        [JsonIgnore(Condition=JsonIgnoreCondition.WhenWritingNull)]
        public WebTask? Task { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
    }
}
