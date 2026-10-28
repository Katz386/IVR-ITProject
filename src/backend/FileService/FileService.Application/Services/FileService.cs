using FileService.Application.DTOs.Response;
using FileService.Application.Interfaces;
using FileService.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Application.Services
{
    public class FileService : IFileService
    {
        private readonly IFileServiceRepository _filesRepository;

        public FileService(IFileServiceRepository filesRepository)
        {
            _filesRepository = filesRepository;
        }

        public async Task<GetFileResponse> GetFile(string id)
        {
            Guid guid;

            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out guid))
                return new GetFileResponse { Message = "Invalid file id", Status = 400 };

            var fileRecord = await _filesRepository.GetFileRecord(guid);

            if (fileRecord == null)
                return new GetFileResponse { Message = "File not found", Status = 404 };

            var content = await _filesRepository.GetFileStream(fileRecord);

            if (content == null)
                return new GetFileResponse { Message = "Something went wrong", Status = 500 };

            return new GetFileResponse { Content = content, Message = "Success!", Name = fileRecord.Name, Status = 200 };
        }

        public async Task<GetFileRecordResponse> GetFileRecord(string id)
        {
            Guid guid;

            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out guid))
                return new GetFileRecordResponse { Message = "Invalid file id", Status = 400 };

            var fileRecord = await _filesRepository.GetFileRecord(guid);

            if (fileRecord == null)
                return new GetFileRecordResponse { Message = "File not found", Status = 404 };

            return new GetFileRecordResponse { File = fileRecord, Message = "Success!", Status = 200 };
        }

        public async Task<UploadFileResponse> UploadFile(IFormFile file, string path = "")
        {
            if (await _filesRepository.FileExists(file.FileName, path))
                return new UploadFileResponse { Message = "File already exists", Status = 400 };

            await using var stream = file.OpenReadStream();

            var response = await _filesRepository.UploadFile(stream, file.FileName, path);

            if (await _filesRepository.FileExists(file.FileName, path))
            {
                FileRecord fileRecord = new FileRecord { Id = Guid.NewGuid(), Name = file.FileName, Path = path };

                if(!await _filesRepository.CreateFileRecord(fileRecord))
                    return new UploadFileResponse { Message = "Internal server error. File record not saved", Status = 500 };

                return new UploadFileResponse { FileRecord = fileRecord, Message = "Success!", Status = 200 };
            }
            else
            {
                return new UploadFileResponse { Message = "File not saved", Status = 500 }; 
            }
        }

        public async Task<UploadFileResponse> UploadTaskFile(IFormFile file)
        {
            return await UploadFile(file, "tasks");
        }
    }
}
