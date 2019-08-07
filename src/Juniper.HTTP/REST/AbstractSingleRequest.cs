using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.Progress;
using Juniper.Serialization;

namespace Juniper.HTTP.REST
{
    public abstract class AbstractSingleRequest<ResponseType> : AbstractRequest<ResponseType, ResponseType>
    {
        private readonly Dictionary<string, List<string>> queryParams = new Dictionary<string, List<string>>();
        private readonly string cacheSubDirectoryName;
        private readonly string path;
        private string acceptType;
        private string extension;
        protected IDeserializer<ResponseType> deserializer;

        protected AbstractSingleRequest(AbstractRequestConfiguration api, IDeserializer<ResponseType> deserializer, string path, string cacheSubDirectoryName)
            : base(api)
        {
            this.deserializer = deserializer;
            this.path = path;
            this.cacheSubDirectoryName = cacheSubDirectoryName;
        }

        protected void SetContentType(string acceptType, string extension)
        {
            this.acceptType = acceptType;
            this.extension = extension;
        }

        protected U SetQuery<U>(string key, U value)
        {
            SetQuery(key, value.ToString());
            return value;
        }

        protected void RemoveQuery(string key)
        {
            queryParams.Remove(key);
        }

        protected bool RemoveQuery(string key, string value)
        {
            var removed = false;
            if (queryParams.ContainsKey(key))
            {
                var list = queryParams[key];
                removed = list.Remove(value);
                if (list.Count == 0)
                {
                    queryParams.Remove(key);
                }
            }

            return removed;
        }

        private string SetQuery(string key, string value, bool allowMany)
        {
            if (value == default && !allowMany)
            {
                RemoveQuery(key);
            }
            else
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

            return value;
        }

        protected string SetQuery(string key, string value)
        {
            return SetQuery(key, value, false);
        }

        protected string AddQuery(string key, string value)
        {
            return SetQuery(key, value, true);
        }

        protected U AddQuery<U>(string key, U value)
        {
            SetQuery(key, value.ToString());
            return value;
        }

        protected bool RemoveQuery<U>(string key, U value)
        {
            return RemoveQuery(key, value.ToString());
        }

        public virtual Uri BaseURI
        {
            get
            {
                var uriBuilder = new UriBuilder(api.baseServiceURI);
                uriBuilder.Path += path;
                uriBuilder.Query = queryParams.ToString("=", "&");
                return uriBuilder.Uri;
            }
        }

        protected virtual Uri AuthenticatedURI
        {
            get
            {
                return BaseURI;
            }
        }

        public FileInfo CacheFile
        {
            get
            {
                var fileName = CacheFileName;
                if (fileName == null)
                {
                    return null;
                }
                else
                {
                    return new FileInfo(fileName);
                }
            }
        }

        protected virtual string CacheFileName
        {
            get
            {
                if (api.cacheLocation == null)
                {
                    return null;
                }
                else
                {
                    var path = Path.Combine(api.cacheLocation.FullName, CacheID);
                    if (!extension.StartsWith("."))
                    {
                        path += ".";
                    }
                    path += extension;
                    return path;
                }
            }
        }

        public virtual string CacheID
        {
            get
            {
                var id = string.Join("_", BaseURI.Query
                    .Substring(1)
                    .Split(Path.GetInvalidFileNameChars()));
                return Path.Combine(cacheSubDirectoryName, id);
            }
        }

        public override int GetHashCode()
        {
            return CacheFileName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null
                && obj is AbstractSingleRequest<ResponseType> req
                && req.CacheFileName == CacheFileName;
        }

        public override bool IsCached
        {
            get
            {
                return CacheFile?.Exists == true;
            }
        }

        private void SetAcceptType(HttpWebRequest request)
        {
            request.Accept = acceptType;
        }

        private Task<T> Get<T>(Func<Stream, T> decoder, IProgress prog)
        {
            return Task.Run(() => HttpWebRequestExt.CachedGet(AuthenticatedURI, decoder, CacheFile, SetAcceptType, prog));
        }

        public override Task<ResponseType> Get(IProgress prog = null)
        {
            return Get(deserializer.Deserialize, prog);
        }

        public override Task Proxy(HttpListenerResponse response)
        {
            return Task.Run(() => HttpWebRequestExt.CachedProxy(response, AuthenticatedURI, CacheFile, SetAcceptType));
        }

        public Task<Stream> GetRaw(IProgress prog = null)
        {
            return Task.Run(() => HttpWebRequestExt.CachedGetRaw(AuthenticatedURI, CacheFile, SetAcceptType, prog));
        }

        public async Task CopyRaw(Stream outStream)
        {
            var inStream = await GetRaw();
            inStream.CopyTo(outStream);
        }
    }
}