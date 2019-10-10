using System.Collections.Generic;

using System.IO;

using Juniper.Compression.Zip;

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

        private string GetCacheFileName<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            var baseName = fileRef.CacheID.Replace('\\', '/');
            var cacheFileName = fileRef.ContentType.AddExtension(baseName);
            return cacheFileName;
        }

        public bool IsCached<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            if (!filesExist.ContainsKey(fileRef.CacheID))
            {
                if (zipFile.Exists)
                {
                    var cacheFileName = GetCacheFileName(fileRef);
                    using (var zip = Decompressor.OpenZip(zipFile))
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

        public Stream Open<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            Stream stream = null;
            if (IsCached(fileRef))
            {
                var cacheFileName = GetCacheFileName(fileRef);
                var zip = Decompressor.OpenZip(zipFile);
                var entry = zip.GetEntry(cacheFileName);
                if (entry != null)
                {
                    stream = new ZipFileEntryStream(zip, entry);
                }
            }
            return stream;
        }
    }
}
