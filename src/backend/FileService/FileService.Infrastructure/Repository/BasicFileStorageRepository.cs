using FileService.Application.Interfaces;
using FileService.Domain.Models;
using FileService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FileService.Infrastructure.Repository
{
    public class BasicFileStorageRepository : IFileServiceRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _basePath;

        public BasicFileStorageRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

            _basePath = _configuration["FileService_Storage"] ?? "Storage";
            Directory.CreateDirectory(_basePath);
        }

        public async Task<bool> CreateFileRecord(FileRecord file)
        {
            try
            {
                _context.Files.Add(file);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\"{ex.Message}\" in CreateFileRecord");
                return false;
            }
        }

        public async Task<bool> FileExists(string fileName, string path)
        {
            return File.Exists(Path.Combine(_basePath, path, fileName));
        }

        public async Task<FileRecord?> GetFileRecord(Guid id)
        {
            try
            {
                return await _context.Files.Where(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\"{ex.Message}\" in GetFileRecord");
                return null;
            }
        }

        public Task<Stream?> GetFileStream(FileRecord file)
        {
            try
            {
                Stream stream = new FileStream(
                    Path.Combine(_basePath, file.Path, file.Name),
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 65536,
                    options: FileOptions.Asynchronous
                    );

                return Task.FromResult(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\"{ex.Message}\" in GetFileStream");
                return Task.FromResult<Stream?>(null);
            }

        }

        public async Task<string> UploadFile(Stream file, string fileName, string path)
        {
            try
            {
                if (!Directory.Exists(Path.Combine(_basePath, path)))
                    Directory.CreateDirectory(Path.Combine(_basePath, path));

                await using var fileStream = new FileStream(
                    Path.Combine(_basePath, path, fileName),
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 65536,
                    useAsync: true
                );

                await file.CopyToAsync(fileStream);

                return Path.Combine(path, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\"{ex.Message}\" in UploadFile");
                return "";
            }
        }
    }
}
