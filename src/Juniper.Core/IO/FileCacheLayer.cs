using System.Collections.Generic;
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

        public Stream Create<MediaTypeT>(IContentReference<MediaTypeT> fileRef, bool overwrite)
            where MediaTypeT : MediaType
        {
            Stream stream = null;
            var file = GetCacheFile(fileRef);
            if (CanCache(fileRef)
                && (overwrite || !file.Exists))
            {
                file.Directory.Create();
                stream = file.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            }

            return stream;
        }

        public virtual Stream Cache<MediaTypeT>(IContentReference<MediaTypeT> fileRef, Stream stream)
            where MediaTypeT : MediaType
        {
            var outStream = Create(fileRef, false);
            return new CachingStream(stream, outStream);
        }

        public Stream Open<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            Stream stream = null;
            var file = GetCacheFile(fileRef);
            if (file.Exists)
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            return stream;
        }

        public IEnumerable<IContentReference<MediaTypeT>> Get<MediaTypeT>(MediaTypeT ofType)
            where MediaTypeT : MediaType
        {
            var q = new Queue<DirectoryInfo>()
            {
                cacheLocation
            };

            while(q.Count > 0)
            {
                var here = q.Dequeue();
                q.AddRange(here.GetDirectories());

                var files = here.GetFiles();
                foreach(var file in files)
                {
                    if(ofType.Matches(file))
                    {
                        var shortName = file.FullName.Substring(cacheLocation.FullName.Length + 1);
                        var cacheID = PathExt.RemoveShortExtension(shortName);
                        yield return new ContentReference<MediaTypeT>(cacheID, ofType);
                    }
                }
            }
        }

        public bool Delete<MediaTypeT>(IContentReference<MediaTypeT> fileRef) where MediaTypeT : MediaType
        {
            var file = GetCacheFile(fileRef);
            return file.TryDelete();
        }
    }
}
