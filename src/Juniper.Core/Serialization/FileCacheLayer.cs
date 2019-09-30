using System.IO;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.HTTP.REST;
using Juniper.Progress;
using Juniper.Streams;

namespace Juniper.Serialization
{
    public class FileCacheLayer : ICacheLayer
    {
        private readonly DirectoryInfo cacheLocation;

        public FileCacheLayer(DirectoryInfo cacheLocation)
        {
            this.cacheLocation = cacheLocation;
        }

        public bool CanWriteCache
        {
            get
            {
                return true;
            }
        }

        public Stream WrapStream(AbstractRequest request, Stream stream)
        {
            var cacheFile = GetCacheFile(request);
            return new CachingStream(stream, cacheFile);
        }

        private FileInfo GetCacheFile(AbstractRequest request)
        {
            var baseName = Path.Combine(cacheLocation.FullName, request.CacheID.RemoveInvalidChars());
            var cacheFileName = request.ContentType
                .AddExtension(baseName);
            var cacheFile = new FileInfo(cacheFileName);
            return cacheFile;
        }

        public bool IsCached(AbstractRequest request)
        {
            return GetCacheFile(request).Exists;
        }

        public Task<Stream> GetStream(AbstractRequest request, IProgress prog)
        {
            Stream stream = null;
            var cacheFile = GetCacheFile(request);
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
