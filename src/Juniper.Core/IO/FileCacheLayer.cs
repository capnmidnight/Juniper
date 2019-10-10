using System.IO;

namespace Juniper.IO
{
    public class FileCacheLayer : ICacheDestinationLayer
    {
        private readonly DirectoryInfo cacheLocation;

        public FileCacheLayer(DirectoryInfo cacheLocation)
        {
            this.cacheLocation = cacheLocation;
        }

        public FileCacheLayer(string directoryName)
            : this(new DirectoryInfo(directoryName))
        { }

        public virtual bool CanCache<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
                return true;
        }

        public bool IsCached<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            return GetCacheFile(fileRef).Exists;
        }

        private FileInfo GetCacheFile<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            var baseName = fileRef.CacheID;
            var relativeName = fileRef.ContentType.AddExtension(baseName);
            var absoluteName = Path.Combine(cacheLocation.FullName, relativeName);
            return new FileInfo(absoluteName);
        }

        public Stream Create<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            var cacheFile = GetCacheFile(fileRef);
            cacheFile.Directory.Create();
            return cacheFile.Open(FileMode.OpenOrCreate, FileAccess.Write);
        }

        public virtual Stream Cache<MediaTypeT>(IContentReference<MediaTypeT> fileRef, Stream stream)
            where MediaTypeT : MediaType
        {
            var outStream = Create(fileRef);
            return new CachingStream(stream, outStream);
        }

        public Stream Open<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            Stream stream = null;
            var file = GetCacheFile(fileRef);
            if (file.Exists)
            {
                stream = file.Open(FileMode.Open, FileAccess.Read);
            }

            return stream;
        }
    }
}
