using Common.FileManager.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Common.FileManager.Interfaces.Implementation
{
    public class LocalFileManager : IFileManagerPapertrail
    {
        private readonly string _basePath;
        private readonly ILogger<LocalFileManager> _logger;

        public LocalFileManager(IConfiguration configuration, ILogger<LocalFileManager> logger)
        {
            _basePath = configuration["FileStorage:BasePath"] ?? Path.Combine(Path.GetTempPath(), "FileStorage");
            _logger = logger;

            // Ensure the base path exists on startup
            if (!Directory.Exists(_basePath))
            {
                try
                {
                    Directory.CreateDirectory(_basePath);
                    _logger.LogInformation($"Base file storage directory created at: {_basePath}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to create base file storage directory at: {_basePath}");
                    // Consider throwing an exception here if the application cannot function without the directory
                }
            }
        }

        public async Task<FileInfoDto?> GetFileInfo(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                _logger.LogWarning("GetFileInfo called with a null or empty file name.");
                return null;
            }

            var path = Path.Combine(_basePath, fileName);
            try
            {
                if (!File.Exists(path))
                {
                    _logger.LogDebug($"File not found at: {path}");
                    return null;
                }

                var info = new FileInfo(path);
                return new FileInfoDto
                {
                    FileName = fileName,
                    FileSize = info.Length,
                    UploadedAt = info.LastWriteTime
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting file info for: {fileName} at {path}");
                return null;
            }
        }

        public async Task<Stream?> GetFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                _logger.LogWarning("GetFile called with a null or empty file name.");
                return null;
            }

            var path = Path.Combine(_basePath, fileName);
            try
            {
                if (!File.Exists(path))
                {
                    _logger.LogDebug($"File not found at: {path}");
                    return null;
                }

                return File.OpenRead(path);
            }
            catch (FileNotFoundException)
            {
                _logger.LogDebug($"File not found at: {path} (FileNotFoundException)");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting file: {fileName} at {path}");
                return null;
            }
        }

        //public async Task DeleteFile(string fileName)
        //{
        //    if (string.IsNullOrWhiteSpace(fileName))
        //    {
        //        _logger.LogWarning("DeleteFile called with a null or empty file name.");
        //        return;
        //    }

        //    var path = Path.Combine(_basePath, fileName);
        //    try
        //    {
        //        if (File.Exists(path))
        //        {
        //            File.Delete(path);
        //            _logger.LogInformation($"File deleted successfully: {path}");
        //        }
        //        else
        //        {
        //            _logger.LogDebug($"Attempted to delete non-existent file: {path}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"An error occurred while deleting file: {fileName} at {path}");
        //        // Consider re-throwing the exception if deletion is critical
        //    }
        //}
        public Task<bool> DeleteFile(string fileName)
        {
            try
            {
                var path = Path.Combine(_basePath, fileName);
                if (File.Exists(path))
                {
                    File.Delete(path);
                    _logger.LogInformation("File deleted: {FileName}", fileName);
                    return Task.FromResult(true);
                }

                _logger.LogWarning("File not found for deletion: {FileName}", fileName);
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file: {FileName}", fileName);
                return Task.FromResult(false);
            }
        }


        public async Task<bool> SaveFile(string fileName, Stream contentStream, IDictionary<string, string>? metadata = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                _logger.LogError("SaveFile called with a null or empty file name.");
                return false;
            }

            if (contentStream == null || contentStream.Length == 0)
            {
                _logger.LogWarning($"SaveFile called with an empty content stream for file: {fileName}");
                return false;
            }

            var path = Path.Combine(_basePath, fileName);
            var directory = Path.GetDirectoryName(path);

            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                    _logger.LogDebug($"Directory created: {directory}");
                }

                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true)) // Added buffer size and async flag
                {
                    await contentStream.CopyToAsync(fileStream);
                    _logger.LogInformation($"File saved successfully to: {path}");
                    return true;
                }

                //// Consider handling metadata here if needed
                //if (metadata != null && metadata.Any())
                //{
                //    // Implement logic to store metadata (e.g., in a separate file or database)
                //    _logger.LogDebug($"Metadata provided for {fileName}: {string.Join(", ", metadata.Select(kv => $"{kv.Key}:{kv.Value}"))}");
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while saving file: {fileName} to {path}");
                return false;
            }
            finally
            {
                // Ensure the content stream is disposed if it's not managed elsewhere
                await contentStream.DisposeAsync();
            }
        }
    }
}
