using Common.FileManager.Models;

namespace Common.FileManager.Interfaces
{
    /// <summary>
    /// Defines a contract for managing files across various storage providers (e.g., local, Azure, S3).
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Retrieves metadata about a file (e.g., size, name, creation timestamp).
        /// </summary>
        /// <param name="filePath">The unique path or name of the file.</param>
        /// <returns>File details if found; otherwise null.</returns>
        Task<FileInfoDto?> GetFileInfo(string filePath);

        /// <summary>
        /// Retrieves the file contents as a stream.
        /// </summary>
        /// <param name="filePath">The path or key to the file in storage.</param>
        /// <returns>Stream of file contents or null if not found.</returns>
        Task<Stream?> GetFile(string filePath);

        /// <summary>
        /// Saves a file to the storage provider.
        /// </summary>
        /// <param name="filePath">Destination file path or name.</param>
        /// <param name="contentStream">File content stream.</param>
        /// <param name="metadata">Optional metadata (e.g., content-type, tags).</param>
        /// <returns>True if saved successfully; false otherwise.</returns>
        Task<bool> SaveFile(string filePath, Stream contentStream, IDictionary<string, string>? metadata = null, bool overwrite = false);

        /// <summary>
        /// Deletes a file from the storage provider.
        /// </summary>
        /// <param name="filePath">Path or key of the file to delete.</param>
        /// <returns>True if deleted; otherwise false.</returns>
        Task<bool> DeleteFile(string filePath);

        /// <summary>
        /// Checks whether the file exists in storage.
        /// </summary>
        /// <param name="filePath">Path of the file to check.</param>
        /// <returns>True if it exists; otherwise false.</returns>
        Task<bool> FileExists(string filePath);

        /// <summary>
        /// Generates a temporary download URL (if supported by provider).
        /// </summary>
        /// <param name="filePath">Path of the file to generate the link for.</param>
        /// <param name="expiryMinutes">Time in minutes before the link expires.</param>
        /// <returns>Signed URL or null if unsupported.</returns>
        Task<string?> GetSignedUrl(string filePath, int expiryMinutes = 60);
    }
}
