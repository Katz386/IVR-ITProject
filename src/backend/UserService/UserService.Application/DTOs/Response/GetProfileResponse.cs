using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using UserService.Domain.Models;

namespace UserService.Application.DTOs.Response
{
    public class GetProfileResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Profile? Profile { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
    }
}
