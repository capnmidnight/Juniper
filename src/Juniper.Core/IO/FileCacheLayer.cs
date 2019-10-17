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

        public virtual bool CanCache(ContentReference fileRef)
        {
            return true;
        }

        public bool IsCached(ContentReference fileRef)
        {
            var file = cacheLocation + fileRef;
            return file.Exists;
        }

        public Stream Create(ContentReference fileRef, bool overwrite)
        {
            Stream stream = null;
            var file = cacheLocation + fileRef;
            if (CanCache(fileRef)
                && (overwrite || !file.Exists))
            {
                file.Directory.Create();
                stream = file.Open(FileMode.Create, FileAccess.Write, FileShare.Write);
            }

            return stream;
        }

        public virtual Stream Cache(ContentReference fileRef, Stream stream)
        {
            var outStream = Create(fileRef, false);
            return new CachingStream(stream, outStream);
        }

        public Stream Open(ContentReference fileRef, IProgress prog)
        {
            Stream stream = null;
            var file = cacheLocation + fileRef;
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

        public IEnumerable<ContentReference> Get<MediaTypeT>(MediaTypeT ofType)
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
                        yield return cacheID + ofType;
                    }
                }
            }
        }

        public bool Delete(ContentReference fileRef)
        {
            var file = cacheLocation + fileRef;
            return file.TryDelete();
        }
    }
}
