using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.Google
{
    public abstract class AbstractSingleSearch<ResultType> : AbstractSearch<ResultType, ResultType>
    {
        private readonly Dictionary<string, List<string>> queryParams = new Dictionary<string, List<string>>();
        private readonly UriBuilder uriBuilder;
        private readonly string cacheLocString;
        private readonly string acceptType;
        private readonly string extension;
        private readonly bool signRequests;

        protected AbstractSingleSearch(Uri baseServiceURI, string path, string cacheLocString, string acceptType, string extension, bool signRequests)
        {
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

        protected void RemoveQuery(string key)
        {
            queryParams.Remove(key);
        }

        private void SetQuery(string key, string value, bool allowMany)
        {
            var list = queryParams.Get(key) ?? new List<string>();
            if (allowMany || list.Count == 0)
            {
                list.Add(value);
            }
            else if (!allowMany)
            {
                list[0] = value;
            }
            queryParams[key] = list;
        }

        protected void SetQuery(string key, string value)
        {
            SetQuery(key, value, false);
        }

        protected void AddQuery(string key, string value)
        {
            SetQuery(key, value, true);
        }

        protected void AddQuery<U>(string key, U value)
        {
            SetQuery(key, value.ToString());
        }

        public virtual Uri Uri
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
