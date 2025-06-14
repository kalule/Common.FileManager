using Common.FileManager.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Common.FileManager.Interfaces.Implementation
{
    public class FileManager : IFileManager
    {
        private readonly string _basePath;
        private readonly ILogger<FileManager> _logger;

        public FileManager(IConfiguration configuration, ILogger<FileManager> logger)
        {
            _basePath = configuration["FileStorage:BasePath"] ?? Path.Combine(Path.GetTempPath(), "FileStorage");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            try
            {
                Directory.CreateDirectory(_basePath);
                _logger.LogInformation("Base file storage directory ensured at: {BasePath}", _basePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to ensure base file storage directory at: {BasePath}", _basePath);
            }
        }

        public async Task<Stream?> GetFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                _logger.LogWarning("GetFile called with an empty filePath.");
                return null;
            }

            var fullPath = Path.Combine(_basePath, filePath);
            try
            {
                return File.Exists(fullPath) ? File.OpenRead(fullPath) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading file: {FilePath}", filePath);
                return null;
            }
        }

        public Task<bool> DeleteFile(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_basePath, filePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("File deleted: {FilePath}", filePath);
                    return Task.FromResult(true);
                }

                _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
                return Task.FromResult(false);
            }
        }

        public async Task<bool> SaveFile(string filePath, Stream contentStream, IDictionary<string, string>? metadata = null, bool overwrite = false)
        {
            if (string.IsNullOrWhiteSpace(filePath) || contentStream == null || contentStream.Length == 0)
            {
                _logger.LogWarning("Invalid filePath or empty stream provided.");
                return false;
            }

            var fullPath = Path.Combine(_basePath, filePath);

            if (File.Exists(fullPath) && !overwrite)
            {
                _logger.LogWarning("File already exists and overwrite is disabled: {FilePath}", filePath);
                return false;
            }

            var directory = Path.GetDirectoryName(fullPath);

            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                    _logger.LogDebug("Directory created: {Directory}", directory);
                }

                using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
                await contentStream.CopyToAsync(fileStream);

                _logger.LogInformation("File saved successfully at: {FilePath}", fullPath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file: {FilePath}", filePath);
                return false;
            }
            finally
            {
                await contentStream.DisposeAsync();
            }
        }

        public async Task<FileInfoDto?> GetFileInfo(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                _logger.LogWarning("GetFileInfo called with an empty filePath.");
                return null;
            }

            var fullPath = Path.Combine(_basePath, filePath);
            try
            {
                if (!File.Exists(fullPath))
                {
                    _logger.LogDebug("File not found: {FilePath}", fullPath);
                    return null;
                }

                var info = new FileInfo(fullPath);
                return new FileInfoDto
                {
                    FileName = filePath,
                    FileSize = info.Length,
                    UploadedAt = info.LastWriteTimeUtc
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving file info for: {FilePath}", filePath);
                return null;
            }
        }

        public Task<bool> FileExists(string filePath)
        {
            var fullPath = Path.Combine(_basePath, filePath);
            return Task.FromResult(File.Exists(fullPath));
        }

        public Task<string?> GetSignedUrl(string filePath, int expiryMinutes = 60)
        {
            _logger.LogWarning("Signed URL generation not supported for local file system storage.");
            return Task.FromResult<string?>(null);
        }
    }
}
