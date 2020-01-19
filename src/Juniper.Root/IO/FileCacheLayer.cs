using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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

        public Stream Create(ContentReference fileRef)
        {
            if (fileRef is null)
            {
                throw new ArgumentNullException(nameof(fileRef));
            }

            if (!CanCache(fileRef))
            {
                throw new ArgumentException("Cannot cache this file.", fileRef.ToString());
            }

            var file = Resolve(fileRef);
            file.Directory.Create();
            return file.Create();
        }

        public virtual Stream Cache(ContentReference fileRef, Stream stream)
        {
            var outStream = Create(fileRef);
            return new CachingStream(stream, outStream);
        }

        public Task<Stream> GetStreamAsync(ContentReference fileRef, IProgress prog)
        {
            if (fileRef is null)
            {
                throw new ArgumentNullException(nameof(fileRef));
            }

            if (!IsCached(fileRef))
            {
                throw new FileNotFoundException("File not found in disk cache!", fileRef.ToString());
            }

            var file = Resolve(fileRef);
            var stream = file.OpenRead();
            var progStream = new ProgressStream(stream, file.Length, prog, true);
            return Task.FromResult((Stream)progStream);
        }

        public IEnumerable<ContentReference> GetContentReferences(MediaType ofType)
        {
            if (ofType is null)
            {
                throw new ArgumentNullException(nameof(ofType));
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
