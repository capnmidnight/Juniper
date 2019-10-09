using System;
using System.Collections.Generic;

using System.IO;

using Juniper.Compression.Zip;

namespace Juniper.IO
{
    public class ZipFileCacheLayer : ICacheLayer
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

        public bool CanCache<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            return false;
        }

        public Stream Cache<MediaTypeT>(IContentReference<MediaTypeT> fileRef, Stream stream)
            where MediaTypeT : MediaType
        {
            return stream;
        }

        public Stream Create<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            throw new NotSupportedException();
        }

        public void Copy<MediaTypeT>(IContentReference<MediaTypeT> fileRef, FileInfo file)
            where MediaTypeT : MediaType
        {
            throw new NotSupportedException();
        }

        protected virtual string GetCacheFileName<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
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
                if (!zipFile.Exists)
                {
                    filesExist[fileRef.CacheID] = false;
                }
                else
                {
                    var cacheFileName = GetCacheFileName(fileRef);
                    using (var zip = Decompressor.OpenZip(zipFile))
                    {
                        var entry = zip.GetEntry(cacheFileName);
                        filesExist[fileRef.CacheID] = entry != null;
                    }
                }
            }

            return filesExist[fileRef.CacheID];
        }

        public IStreamSource<MediaTypeT> GetCachedSource<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            return new ZipFileReference<MediaTypeT>(this, GetCacheFileName(fileRef), fileRef);
        }
    }
}
