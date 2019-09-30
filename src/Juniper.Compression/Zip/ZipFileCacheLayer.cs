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

        public Stream WrapStream(string fileDescriptor, MediaType contentType, Stream stream)
        {
            return stream;
        }

        protected virtual string GetCacheFileName(string fileDescriptor, MediaType contentType)
        {
            var baseName = PathExt.FixPath(fileDescriptor);
            var cacheFileName = contentType.AddExtension(baseName);
            return cacheFileName;
        }

        public bool IsCached(string fileDescriptor, MediaType contentType)
        {
            var cacheFileName = GetCacheFileName(fileDescriptor, contentType);
            using (var zip = Decompressor.OpenZip(zipFile))
            {
                var entry = zip.GetEntry(cacheFileName);
                return entry != null;
            }
        }

        public Task<Stream> GetStream(string fileDescriptor, MediaType contentType, IProgress prog)
        {
            Stream stream = null;
            var cacheFileName = GetCacheFileName(fileDescriptor, contentType);
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
