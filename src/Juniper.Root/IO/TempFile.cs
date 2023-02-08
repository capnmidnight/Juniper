namespace Juniper.IO
{
    public class TempFile : IDisposable
    {
        private bool disposedValue;

        public string FilePath { get; }

        public MediaType MediaType { get; }

        public TempFile(MediaType mediaType = null)
        {
            FilePath = Path.GetTempFileName();
            MediaType = mediaType;
            if (mediaType is not null)
            {
                FilePath = mediaType.AddExtension(FilePath);
            }
        }

        public async Task WriteAsync(Stream stream)
        {
            using var fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream);
            await fileStream.FlushAsync();
        }

        public Stream Open()
        {
            return File.OpenRead(FilePath);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    File.Delete(FilePath);
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}