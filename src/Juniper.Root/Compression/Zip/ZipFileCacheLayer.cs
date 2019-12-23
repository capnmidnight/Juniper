using System.Collections.Generic;

using System.IO;
using System.Threading.Tasks;

using Juniper.Compression;
using Juniper.Compression.Zip;
using Juniper.Progress;

namespace Juniper.IO
{
    public class ZipFileCacheLayer : ICacheSourceLayer
    {
        internal readonly FileInfo zipFile;

        private readonly Dictionary<string, bool> filesExist = new Dictionary<string, bool>();

        public ZipFileCacheLayer(FileInfo zipFile)
        {
            this.zipFile = zipFile;

            if (!zipFile.Exists)
            {
                throw new FileNotFoundException("ZipFileCacheLayer: No zip file! " + zipFile.FullName);
            }
        }

        public ZipFileCacheLayer(string fileName)
            : this(new FileInfo(fileName))
        { }

        private string GetCacheFileName(ContentReference fileRef)
        {
            var baseName = fileRef.CacheID.Replace('\\', '/');
            var cacheFileName = fileRef.ContentType.AddExtension(baseName);
            return cacheFileName;
        }

        public bool IsCached(ContentReference fileRef)
        {
            if (!filesExist.ContainsKey(fileRef.CacheID))
            {
                if (zipFile.Exists)
                {
                    var cacheFileName = GetCacheFileName(fileRef);
                    using (var zip = Decompressor.Open(zipFile))
                    {
                        var entry = zip.GetEntry(cacheFileName);
                        filesExist[fileRef.CacheID] = entry != null;
                    }
                }
                else
                {
                    filesExist[fileRef.CacheID] = false;
                }
            }

            return filesExist.ContainsKey(fileRef.CacheID)
                && filesExist[fileRef.CacheID];
        }

        public Task<Stream> GetStreamAsync(ContentReference fileRef, IProgress prog)
        {
            Stream stream = null;
            if (IsCached(fileRef))
            {
                var zip = Decompressor.Open(zipFile);
                var cacheFileName = GetCacheFileName(fileRef);
                stream = zip.GetFile(cacheFileName);
            }

            return Task.FromResult(stream);
        }

        public IEnumerable<ContentReference> GetContentReference(MediaType ofType)
        {
            foreach (var file in Decompressor.Entries(zipFile).Files())
            {
                if (ofType.Matches(file.FullName))
                {
                    var cacheID = PathExt.RemoveShortExtension(file.FullName);
                    yield return cacheID + ofType;
                }
            }
        }
    }
}
