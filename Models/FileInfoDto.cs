namespace Common.FileManager.Models
{
    public class FileInfoDto
    {
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
