using System.IO;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.Progress;

namespace Juniper.Caching
{
    public class FileCacheLayer : ICacheLayer
    {
        private readonly DirectoryInfo cacheLocation;

        public FileCacheLayer(DirectoryInfo cacheLocation)
        {
            this.cacheLocation = cacheLocation;
        }

        public FileCacheLayer(string directoryName)
            : this(new DirectoryInfo(directoryName))
        { }

        public bool CanCache
        {
            get
            {
                return true;
            }
        }

        public Stream WrapStream(string fileDescriptor, MediaType contentType, Stream stream)
        {
            var cacheFile = GetCacheFile(fileDescriptor, contentType);
            return new CachingStream(stream, cacheFile);
        }

        public Stream OpenWrite(string fileDescriptor, MediaType contentType)
        {
            var cacheFile = GetCacheFile(fileDescriptor, contentType);
            cacheFile.Directory.Create();
            return cacheFile.OpenWrite();
        }

        public void Copy(FileInfo file, string fileDescriptor, MediaType contentType)
        {
            var cacheFile = GetCacheFile(fileDescriptor, contentType);
            cacheFile.Directory.Create();
            File.Copy(file.FullName, cacheFile.FullName, true);
        }

        private FileInfo GetCacheFile(string fileDescriptor, MediaType contentType)
        {
            return new FileInfo(GetCacheFileName(fileDescriptor, contentType));
        }

        protected virtual string GetCacheFileName(string fileDescriptor, MediaType contentType)
        {
            var baseName = PathExt.FixPath(fileDescriptor);
            var cacheFileName = contentType.AddExtension(baseName);
            return Path.Combine(cacheLocation.FullName, cacheFileName);
        }

        public bool IsCached(string fileDescriptor, MediaType contentType)
        {
            return GetCacheFile(fileDescriptor, contentType).Exists;
        }

        public Task<Stream> GetStream(string fileDescriptor, MediaType contentType, IProgress prog)
        {
            Stream stream = null;
            var cacheFile = GetCacheFile(fileDescriptor, contentType);
            if (cacheFile.Exists)
            {
                stream = cacheFile.OpenRead();
                if (prog != null)
                {
                    var length = cacheFile.Length;
                    stream = new ProgressStream(stream, length, prog);
                }
            }

            return Task.FromResult(stream);
        }
    }
}
