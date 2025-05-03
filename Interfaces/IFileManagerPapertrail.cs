namespace Common.FileManager.Interfaces
{
    public interface IFileManagerPapertrail : IFileManager
    {
        Task<bool> SaveFile(string fileName, Stream contentStream, IDictionary<string, string>? metadata = null);
    }

}
