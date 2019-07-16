using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.Google
{
    public abstract class AbstractSingleSearch<T> : AbstractSearch<T, T>
    {
        protected readonly UriBuilder uriBuilder;
        private readonly string cacheLocString;
        private readonly string extension;

        protected AbstractSingleSearch(Uri baseServiceURI, string path, string cacheLocString, string extension)
        {
            uriBuilder = new UriBuilder(baseServiceURI);
            uriBuilder.Path += path;
            this.cacheLocString = cacheLocString;
            this.extension = extension;
        }

        public Uri Uri
        {
            get
            {
                return uriBuilder.Uri;
            }
        }

        private FileInfo GetCacheFile(AbstractAPI api)
        {
            var cacheID = string.Join("_", Uri.PathAndQuery
                                            .Split(Path.GetInvalidFileNameChars()));
            var path = Path.Combine(api.cacheLocation.FullName, cacheLocString, cacheID);
            if (!extension.StartsWith("."))
            {
                path += ".";
            }
            path += extension;
            var file = new FileInfo(path);
            return file;
        }

        internal override bool IsCached(AbstractAPI api)
        {
            return GetCacheFile(api).Exists;
        }

        internal abstract Func<Stream, T> GetDecoder(AbstractAPI api);

        internal override Task<T> Get(AbstractAPI api)
        {
            var file = GetCacheFile(api);
            return Task.Run(() => HttpWebRequestExt.CachedGet(
                api.Sign(Uri),
                GetDecoder(api),
                file));
        }
    }
}
