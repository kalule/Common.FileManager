namespace Common.FileManager.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<byte[]> ReadBytesFromStreamAsync(this Stream stream)
        {
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }

        public static Stream ConvertToStream(this byte[] bytes)
        {
            return new MemoryStream(bytes);
        }
    }
}
