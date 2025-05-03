using Common.FileManager.Models;

namespace Common.FileManager.Interfaces
{
    public interface IFileManager
    {
        Task<FileInfoDto?> GetFileInfo(string fileName);
        Task<Stream?> GetFile(string fileName);
        //Task DeleteFile(string fileName);

        Task<bool> DeleteFile(string fileName);
    }
}
