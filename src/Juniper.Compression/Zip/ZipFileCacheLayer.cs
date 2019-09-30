using System.IO;
using System.Threading.Tasks;
using Juniper.HTTP;
using Juniper.HTTP.REST;
using Juniper.Progress;
using Juniper.Serialization;
using Juniper.Streams;

namespace Juniper.Compression.Zip
{
    public class ZipFileCacheLayer : ICacheLayer
    {
        private readonly FileInfo zipFile;

        public ZipFileCacheLayer(FileInfo zipFile)
        {
            this.zipFile = zipFile;
        }

        public ZipFileCacheLayer(string fileName)
            : this(new FileInfo(fileName))
        { }

        public Stream WrapStream(AbstractRequest request, Stream stream)
        {
            return stream;
        }

        private static string GetCacheFileName(AbstractRequest request)
        {
            var baseName = request.CacheID.RemoveInvalidChars();
            var cacheFileName = request.ContentType.AddExtension(baseName);
            return cacheFileName;
        }

        public bool IsCached(AbstractRequest request)
        {
            var cacheFileName = GetCacheFileName(request);
            using (var zip = Decompressor.OpenZip(zipFile))
            {
                var entry = zip.GetEntry(cacheFileName);
                return entry != null;
            }
        }

        public Task<Stream> GetStream(AbstractRequest request, IProgress prog)
        {
            Stream stream = null;
            var cacheFileName = GetCacheFileName(request);
            var zip = Decompressor.OpenZip(zipFile);
            var entry = zip.GetEntry(cacheFileName);
            if (entry != null)
            {
                stream = new ZipFileEntryStream(zip, entry, prog);
                if (prog != null)
                {
                    var length = entry.Size;
                    stream = new ProgressStream(stream, length, prog);
                }
            }
            return Task.FromResult(stream);
        }
    }
}
