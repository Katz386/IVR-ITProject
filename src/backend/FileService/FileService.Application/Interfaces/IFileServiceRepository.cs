using FileService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Application.Interfaces
{
    public interface IFileServiceRepository
    {
        public Task<Stream?> GetFileStream(FileRecord file);
        public Task<FileRecord?> GetFileRecord(Guid id);
        public Task<string> UploadFile(Stream file, string fileName, string path);
        public Task<bool> CreateFileRecord(FileRecord file);
        public Task<bool> FileExists(string fileName, string path);
    }
}
