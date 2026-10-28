using FileService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Application.DTOs.Response
{
    public class UploadFileResponse
    {
        public FileRecord? FileRecord { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
    }
}
