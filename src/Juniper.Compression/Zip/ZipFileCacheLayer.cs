using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.Caching;
using Juniper.HTTP;
using Juniper.Progress;

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

        public bool CanCache
        {
            get
            {
                return false;
            }
        }

        public Stream WrapStream(string fileDescriptor, MediaType contentType, Stream stream)
        {
            return stream;
        }

        public Stream OpenWrite(string fileDescriptor, MediaType contentType)
        {
            throw new NotSupportedException();
        }

        public void Copy(FileInfo file, string fileDescriptor, MediaType contentType)
        {
            throw new NotSupportedException();
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
