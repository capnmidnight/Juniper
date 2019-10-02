using System;
using System.IO;
using Juniper.HTTP;
using Juniper.IO;

namespace Juniper.Compression.Zip
{
    public class ZipFileCacheLayer : ICacheLayer
    {
        internal readonly FileInfo zipFile;

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

        public Stream Cache<MediaTypeT>(IContentReference<MediaTypeT> source, Stream stream)
            where MediaTypeT : MediaType
        {
            return stream;
        }

        public Stream OpenWrite<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            throw new NotSupportedException();
        }

        public void Copy<MediaTypeT>(IContentReference<MediaTypeT> source, FileInfo file)
            where MediaTypeT : MediaType
        {
            throw new NotSupportedException();
        }

        protected virtual string GetCacheFileName<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            var baseName = PathExt.FixPath(source.CacheID);
            var cacheFileName = source.ContentType.AddExtension(baseName);
            return cacheFileName;
        }

        public bool IsCached<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            var cacheFileName = GetCacheFileName(source);
            using (var zip = Decompressor.OpenZip(zipFile))
            {
                var entry = zip.GetEntry(cacheFileName);
                return entry != null;
            }
        }

        public IStreamSource<MediaTypeT> GetCachedSource<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            return new ZipFileReference<MediaTypeT>(this, GetCacheFileName(source), source);
        }
    }
}
