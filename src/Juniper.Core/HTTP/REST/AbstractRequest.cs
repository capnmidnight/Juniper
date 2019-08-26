using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.Progress;
using Juniper.Serialization;
using Juniper.Streams;

namespace Juniper.HTTP.REST
{
    public abstract class AbstractRequest
    {
        private readonly AbstractRequestConfiguration api;
        private readonly IDictionary<string, List<string>> queryParams = new SortedDictionary<string, List<string>>();
        private readonly string cacheSubDirectoryName;
        private readonly string path;

        protected AbstractRequest(AbstractRequestConfiguration api, string path, string cacheSubDirectoryName = null)
        {
            this.api = api;
            this.path = path;
            this.cacheSubDirectoryName = cacheSubDirectoryName;
        }

        private void SetQuery(string key, string value, bool allowMany)
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
        }

        protected void SetQuery(string key, string value)
        {
            SetQuery(key, value, false);
        }

        protected void SetQuery<U>(string key, U value)
        {
            SetQuery(key, value.ToString());
        }

        protected void AddQuery(string key, string value)
        {
            SetQuery(key, value, true);
        }

        protected void AddQuery<U>(string key, U value)
        {
            SetQuery(key, value.ToString());
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
                    return Path.Combine(api.cacheLocation.FullName, CacheID);
                }
            }
        }

        public virtual string CacheID
        {
            get
            {
                string id = string.Join("_", BaseURI.PathAndQuery
                    .Substring(1)
                    .Split(Path.GetInvalidFileNameChars()));
                if (cacheSubDirectoryName == null)
                {
                    return id;
                }
                else
                {
                    return Path.Combine(cacheSubDirectoryName, id);
                }
            }
        }

        public override int GetHashCode()
        {
            return CacheFileName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null
                && obj is AbstractRequest req
                && req.CacheFileName == CacheFileName;
        }

        public async Task Proxy(HttpListenerResponse outResponse, MediaType acceptType = null)
        {
            var cacheFileName = CacheFileName;
            if (acceptType != null)
            {
                if (acceptType.PrimaryExtension != null)
                {
                    cacheFileName += ".";
                    cacheFileName += acceptType.PrimaryExtension;
                }
            }

            var cacheFile = new FileInfo(cacheFileName);

            if (cacheFile != null
                && File.Exists(cacheFile.FullName)
                && cacheFile.Length > 0)
            {
                outResponse.SetStatus(HttpStatusCode.OK);
                outResponse.ContentType = cacheFile.GetContentType();
                outResponse.ContentLength64 = cacheFile.Length;
                outResponse.SendFile(cacheFile);
            }
            else
            {
                var request = HttpWebRequestExt.Create(AuthenticatedURI);
                if (acceptType != null)
                {
                    request.Accept = acceptType;
                }

                using (var inResponse = await request.Get())
                {
                    var body = inResponse.GetResponseStream();
                    if (cacheFile != null)
                    {
                        body = new CachingStream(body, cacheFile);
                    }
                    using (body)
                    {
                        outResponse.SetStatus(inResponse.StatusCode);
                        outResponse.ContentType = inResponse.ContentType;
                        if (inResponse.ContentLength >= 0)
                        {
                            outResponse.ContentLength64 = inResponse.ContentLength;
                        }
                        body.CopyTo(outResponse.OutputStream);
                    }
                }
            }
        }

        public async Task<Stream> GetStream(MediaType acceptType = null, IProgress prog = null)
        {
            Stream body;
            long length;

            var cacheFileName = CacheFileName;
            if (acceptType?.PrimaryExtension != null)
            {
                var expectedExt = "." + acceptType.PrimaryExtension;
                if (Path.GetExtension(cacheFileName) != expectedExt)
                {
                    cacheFileName += expectedExt;
                }
            }

            var cacheFile = new FileInfo(cacheFileName);

            if (cacheFile != null
                && File.Exists(cacheFile.FullName)
                && cacheFile.Length > 0)
            {
                length = cacheFile.Length;
                body = cacheFile.OpenRead();
            }
            else
            {
                var request = HttpWebRequestExt.Create(AuthenticatedURI);
                if (acceptType != null)
                {
                    request.Accept = acceptType;
                }

                var response = await request.Get();
                length = response.ContentLength;
                body = response.GetResponseStream();
                if (cacheFile != null)
                {
                    body = new CachingStream(body, cacheFile);
                }
            }

            return new ProgressStream(body, length, prog);
        }

        public async Task<T> GetDecoded<T>(IDeserializer<T> deserializer, IProgress prog = null)
        {
            using (var stream = await GetStream(deserializer.ContentType, prog))
            {
                return deserializer.Deserialize(stream);
            }
        }
    }
}