using System.IO;

using Juniper.HTTP;

namespace Juniper.IO
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

        public Stream Cache<MediaTypeT>(IContentReference<MediaTypeT> source, Stream stream)
            where MediaTypeT : MediaType
        {
            var cacheFile = GetCacheFile(source);
            return new CachingStream(stream, cacheFile);
        }

        public Stream OpenWrite<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            var cacheFile = GetCacheFile(source);
            cacheFile.Directory.Create();
            return cacheFile.OpenWrite();
        }

        public void Copy<MediaTypeT>(IContentReference<MediaTypeT> source, FileInfo file)
            where MediaTypeT : MediaType
        {
            var cacheFile = GetCacheFile(source);
            cacheFile.Directory.Create();
            File.Copy(file.FullName, cacheFile.FullName, true);
        }

        internal FileInfo GetCacheFile<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            return new FileInfo(GetCacheFileName(source));
        }

        protected virtual string GetCacheFileName<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            var baseName = PathExt.FixPath(source.CacheID);
            var cacheFileName = source.ContentType.AddExtension(baseName);
            return Path.Combine(cacheLocation.FullName, cacheFileName);
        }

        public bool IsCached<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            return GetCacheFile(source).Exists;
        }

        public IStreamSource<MediaTypeT> GetStreamSource<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            return new FileReference<MediaTypeT>(GetCacheFile(source), source.CacheID, source.ContentType);
        }
    }
}
