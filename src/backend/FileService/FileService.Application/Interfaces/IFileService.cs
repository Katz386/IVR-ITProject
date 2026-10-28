using FileService.Application.DTOs.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Application.Interfaces
{
    public interface IFileService
    {
        public Task<GetFileResponse> GetFile(string id);
        public Task<UploadFileResponse> UploadFile(IFormFile file, string path = "");
        public Task<GetFileRecordResponse> GetFileRecord(string id);
        public Task<UploadFileResponse> UploadTaskFile(IFormFile file);
    }
}
