using Juniper.Compression;
using Juniper.Compression.Zip;
using Juniper.Progress;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Juniper.IO
{
    public class ZipFileCacheLayer : ICacheSourceLayer
    {
        internal readonly FileInfo zipFile;

        private readonly Dictionary<string, bool> filesExist = new Dictionary<string, bool>();

        public ZipFileCacheLayer(FileInfo zipFile)
        {
            if (zipFile is null)
            {
                throw new ArgumentNullException(nameof(zipFile));
            }

            if (!zipFile.Exists)
            {
                throw new FileNotFoundException("ZipFileCacheLayer: No zip file! " + zipFile.FullName);
            }

            this.zipFile = zipFile;
        }

        public ZipFileCacheLayer(string fileName)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName.Length == 0)
            {
                throw new ArgumentException("path must not be empty string", nameof(fileName));
            }

            zipFile = new FileInfo(fileName);

            if (!zipFile.Exists)
            {
                throw new FileNotFoundException("ZipFileCacheLayer: No zip file! " + zipFile.FullName);
            }
        }

        private static string GetCacheFileName(ContentReference fileRef)
        {
            var baseName = fileRef.CacheID.Replace('\\', '/');
            var cacheFileName = baseName?.AddExtension(fileRef.ContentType);
            return cacheFileName;
        }

        public bool IsCached(ContentReference fileRef)
        {
            if (fileRef is null)
            {
                throw new ArgumentNullException(nameof(fileRef));
            }

            if (!filesExist.ContainsKey(fileRef.CacheID))
            {
                if (zipFile.Exists)
                {
                    var cacheFileName = GetCacheFileName(fileRef);
                    using var zip = Decompressor.Open(zipFile);
                    var entry = zip.GetEntry(cacheFileName);
                    filesExist[fileRef.CacheID] = entry is object;
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
            if (fileRef is null)
            {
                throw new ArgumentNullException(nameof(fileRef));
            }

            Stream stream = null;
            if (IsCached(fileRef))
            {
                var cacheFileName = GetCacheFileName(fileRef);
                stream = Decompressor.GetFile(zipFile, cacheFileName);
            }

            return Task.FromResult(stream);
        }

        public IEnumerable<ContentReference> GetContentReferences(MediaType ofType)
        {
            if (ofType is null)
            {
                throw new ArgumentNullException(nameof(ofType));
            }

            foreach (var file in Decompressor.Entries(zipFile).Files())
            {
                if (ofType.GuessMatches(file.FullName))
                {
                    var cacheID = PathExt.RemoveShortExtension(file.FullName);
                    yield return cacheID + ofType;
                }
            }
        }
    }
}
