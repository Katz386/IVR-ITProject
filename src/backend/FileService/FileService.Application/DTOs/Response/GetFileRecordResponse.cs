using FileService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FileService.Application.DTOs.Response
{
    public class GetFileRecordResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FileRecord? File { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
    }
}
