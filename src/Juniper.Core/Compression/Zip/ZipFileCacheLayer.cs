using System.Collections.Generic;

using System.IO;
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

        public Stream Open(ContentReference fileRef, IProgress prog)
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

                    if(prog != null)
                    {
                        stream = new ProgressStream(stream, entry.Size, prog);
                    }
                }
            }
            return stream;
        }

        public IEnumerable<ContentReference> Get<MediaTypeT>(MediaTypeT ofType)
            where MediaTypeT : MediaType
        {
            foreach (var file in Decompressor.Entries(zipFile).Files())
            {
                var fileType = (MediaType)file.Name;
                if (fileType is MediaTypeT mediaType)
                {
                    var cacheID = PathExt.RemoveShortExtension(file.Name);
                    yield return cacheID + mediaType;
                }
            }
        }
    }
}
