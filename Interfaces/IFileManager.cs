using Common.FileManager.Models;

namespace Common.FileManager.Interfaces
{
    /// <summary>
    /// Defines operations for managing files in a storage provider.
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Retrieves metadata about a file (e.g., size, name, timestamp) by file name.
        /// </summary>
        /// <param name="fileName">The name of the file to retrieve metadata for.</param>
        /// <returns>A <see cref="FileInfoDto"/> containing file details, or null if the file does not exist.</returns>
        Task<FileInfoDto?> GetFileInfo(string fileName);

        /// <summary>
        /// Retrieves a file as a stream from the storage provider.
        /// </summary>
        /// <param name="fileName">The name of the file to retrieve.</param>
        /// <returns>A <see cref="Stream"/> of the file contents, or null if the file is not found.</returns>
        Task<Stream?> GetFile(string fileName);

        /// <summary>
        /// Deletes a file from the storage provider.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <returns>True if the file was deleted successfully; false otherwise.</returns>
        Task<bool> DeleteFile(string fileName);

        /// <summary>
        /// Saves a file to the storage provider with optional metadata.
        /// </summary>
        Task<bool> SaveFile(string fileName, Stream contentStream, IDictionary<string, string>? metadata = null);
    }
}