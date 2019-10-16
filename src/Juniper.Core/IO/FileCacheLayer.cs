using System.Collections.Generic;
using System.IO;

using Juniper.Progress;

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

        public virtual bool CanCache(IContentReference fileRef)
        {
            return true;
        }

        public bool IsCached(IContentReference fileRef)
        {
            return GetCacheFile(fileRef).Exists;
        }

        private FileInfo GetCacheFile(IContentReference fileRef)
        {
            var baseName = fileRef.CacheID;
            var relativeName = fileRef.ContentType.AddExtension(baseName);
            var absoluteName = Path.Combine(cacheLocation.FullName, relativeName);
            return new FileInfo(absoluteName);
        }

        public Stream Create(IContentReference fileRef, bool overwrite)
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

        public virtual Stream Cache(IContentReference fileRef, Stream stream)
        {
            var outStream = Create(fileRef, false);
            return new CachingStream(stream, outStream);
        }

        public Stream Open(IContentReference fileRef, IProgress prog)
        {
            Stream stream = null;
            var file = GetCacheFile(fileRef);
            if (file.Exists)
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);

                if (prog != null)
                {
                    stream = new ProgressStream(stream, file.Length, prog);
                }
            }

            return stream;
        }

        public IEnumerable<IContentReference> Get<MediaTypeT>(MediaTypeT ofType)
            where MediaTypeT : MediaType
        {
            var q = new Queue<DirectoryInfo>()
            {
                cacheLocation
            };

            while (q.Count > 0)
            {
                var here = q.Dequeue();
                q.AddRange(here.GetDirectories());

                var files = Directory.GetFiles(here.FullName);
                foreach (var file in files)
                {
                    if (ofType.Matches(file))
                    {
                        var shortName = file.Substring(cacheLocation.FullName.Length + 1);
                        var cacheID = PathExt.RemoveShortExtension(shortName);
                        yield return cacheID.ToRef(ofType);
                    }
                }
            }
        }

        public bool Delete(IContentReference fileRef)
        {
            var file = GetCacheFile(fileRef);
            return file.TryDelete();
        }
    }
}
