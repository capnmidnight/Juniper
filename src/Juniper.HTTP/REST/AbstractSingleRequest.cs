using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP.REST
{
    public abstract class AbstractSingleRequest<ResponseType> : AbstractRequest<ResponseType, ResponseType>
    {
        private readonly Dictionary<string, List<string>> queryParams = new Dictionary<string, List<string>>();
        private readonly UriBuilder uriBuilder;
        private readonly string cacheLocString;
        private readonly string acceptType;
        private readonly string extension;

        protected AbstractSingleRequest(Uri baseServiceURI, string path, string cacheLocString, string acceptType, string extension)
        {
            uriBuilder = new UriBuilder(baseServiceURI);
            uriBuilder.Path += path;
            this.cacheLocString = cacheLocString;
            this.acceptType = acceptType;
            this.extension = extension;
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

        public virtual Uri BaseURI
        {
            get
            {
                uriBuilder.Query = queryParams.ToString("=", "&");
                return uriBuilder.Uri;
            }
        }

        protected virtual Uri MakeAuthenticatedURI(AbstractEndpoint api)
        {
            return BaseURI;
        }

        private FileInfo GetCacheFile(AbstractEndpoint api)
        {
            var cacheID = string.Join("_", BaseURI.PathAndQuery
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

        public override bool IsCached(AbstractEndpoint api)
        {
            return GetCacheFile(api).Exists;
        }

        public abstract Func<Stream, ResponseType> GetDecoder(AbstractEndpoint api);

        private void SetAcceptType(HttpWebRequest request)
        {
            request.Accept = acceptType;
        }

        private Task<T> Get<T>(AbstractEndpoint api, Func<Stream, T> decoder)
        {
            var uri = MakeAuthenticatedURI(api);
            var file = GetCacheFile(api);
            return Task.Run(() => HttpWebRequestExt.CachedGet(uri, decoder, file, SetAcceptType));
        }

        public override Task<ResponseType> Get(AbstractEndpoint api)
        {
            return Get(api, GetDecoder(api));
        }

        public Task<Stream> GetRaw(AbstractEndpoint api)
        {
            return Get(api, stream => stream);
        }
    }
}