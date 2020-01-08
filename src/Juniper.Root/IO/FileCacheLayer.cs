using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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

        /// <summary>
        /// Converts a ContentReference to a FileInfo as if the file exists
        /// in this cache layer. Check FileInfo.Exists to make sure the file
        /// actually exists.
        /// </summary>
        /// <param name="fileRef"></param>
        /// <returns></returns>
        public FileInfo Resolve(ContentReference fileRef)
        {
            return cacheLocation + fileRef;
        }

        public bool IsCached(ContentReference fileRef)
        {
            return Resolve(fileRef).Exists;
        }

        public Stream Create(ContentReference fileRef, bool overwrite)
        {
            Stream stream = null;
            var file = Resolve(fileRef);
            if (CanCache(fileRef)
                && (overwrite || !file.Exists))
            {
                file.Directory.Create();
                stream = file.Open(FileMode.Create, FileAccess.Write, FileShare.None);
            }

            return stream;
        }

        public virtual Stream Cache(ContentReference fileRef, Stream stream)
        {
            var outStream = Create(fileRef, false);
            return new CachingStream(stream, outStream);
        }

        public Task<Stream> GetStreamAsync(ContentReference fileRef, IProgress prog)
        {
            Stream stream = null;
            var file = Resolve(fileRef);
            if (file.Exists)
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);

                if (prog is object)
                {
                    stream = new ProgressStream(stream, file.Length, prog);
                }
            }

            return Task.FromResult(stream);
        }

        public IEnumerable<ContentReference> GetContentReferences(MediaType ofType)
        {
            if (ofType is null)
            {
                throw new System.ArgumentNullException(nameof(ofType));
            }

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
            return Resolve(fileRef).TryDelete();
        }
    }
}
