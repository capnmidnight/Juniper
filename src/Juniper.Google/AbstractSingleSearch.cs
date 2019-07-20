using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.Google
{
    public abstract class AbstractSingleSearch<ResultType> : AbstractSearch<ResultType, ResultType>
    {
        private readonly Dictionary<string, string> queryParams;
        private readonly UriBuilder uriBuilder;
        private readonly string cacheLocString;
        private readonly string acceptType;
        private readonly string extension;
        private readonly bool signRequests;

        protected AbstractSingleSearch(Uri baseServiceURI, string path, string cacheLocString, string acceptType, string extension, bool signRequests)
        {
            queryParams = new Dictionary<string, string>();
            uriBuilder = new UriBuilder(baseServiceURI);
            uriBuilder.Path += path;
            this.cacheLocString = cacheLocString;
            this.acceptType = acceptType;
            this.extension = extension;
            this.signRequests = signRequests;
        }

        protected void SetQuery<U>(string key, U value)
        {
            SetQuery(key, value.ToString());
        }

        protected void SetQuery(string key, string value)
        {
            queryParams[key] = value;
        }

        public Uri Uri
        {
            get
            {
                uriBuilder.Query = queryParams.ToString("=", "&");
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

        internal abstract Func<Stream, ResultType> GetDecoder(AbstractAPI api);

        private void SetAcceptType(HttpWebRequest request)
        {
            request.Accept = acceptType;
        }

        internal override Task<ResultType> Get(AbstractAPI api)
        {
            var uri = api.AddCredentials(Uri, signRequests);
            var decoder = GetDecoder(api);
            var file = GetCacheFile(api);
            return Task.Run(() => HttpWebRequestExt.CachedGet(uri, decoder, file, SetAcceptType));
        }
    }
}
