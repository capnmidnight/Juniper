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
        protected static Uri AddPath(Uri baseURI, string path)
        {
            var uriBuilder = new UriBuilder(baseURI);
            uriBuilder.Path = Path.Combine(uriBuilder.Path, path);
            return uriBuilder.Uri;
        }

        protected static DirectoryInfo AddPath(DirectoryInfo baseDirectory, string path)
        {
            if (baseDirectory == null || string.IsNullOrEmpty(path))
            {
                return baseDirectory;
            }
            else
            {
                var newPath = Path.Combine(baseDirectory.FullName, path);
                return new DirectoryInfo(newPath);
            }
        }

        private readonly Uri serviceURI;
        private readonly DirectoryInfo cacheLocation;
        private readonly IDictionary<string, List<string>> queryParams =
            new SortedDictionary<string, List<string>>();

        protected AbstractRequest(Uri serviceURI, DirectoryInfo cacheLocation)
        {
            this.serviceURI = serviceURI;
            this.cacheLocation = cacheLocation;
            this.cacheLocation?.Create();
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
                var uriBuilder = new UriBuilder(serviceURI);
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
                if (cacheLocation == null)
                {
                    return null;
                }
                else
                {
                    return Path.Combine(cacheLocation.FullName, CacheID);
                }
            }
        }

        public virtual string CacheID
        {
            get
            {
                return BaseURI.PathAndQuery.Substring(1).RemoveInvalidChars();
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

        protected virtual async Task ModifyRequest(HttpWebRequest request)
        { }

        private async Task<HttpWebRequest> CreateRequest(MediaType acceptType)
        {
            var request = HttpWebRequestExt.Create(AuthenticatedURI);
            await ModifyRequest(request);

            if (acceptType != null)
            {
                request.Accept = acceptType;
            }

            return request;
        }

        protected virtual BodyInfo GetBodyInfo() { return null; }

        protected virtual void WriteBody(Stream stream) { }

        private async Task<Stream> OpenCachedStream(MediaType acceptType, Func<MediaType, IProgress, Task<HttpWebResponse>> action, IProgress prog)
        {
            Stream body;
            long length;
            var cacheFile = GetCacheFileName(acceptType);

            if (cacheFile != null
                && File.Exists(cacheFile.FullName)
                && cacheFile.Length > 0)
            {
                length = cacheFile.Length;
                body = cacheFile.OpenRead();
            }
            else
            {
                var progs = prog.Split("Get", "Read");
                prog = progs[1];
                var response = await action(acceptType, progs[0]);
                length = response.ContentLength;
                body = response.GetResponseStream();
                if (cacheFile != null)
                {
                    body = new CachingStream(body, cacheFile);
                }
            }

            return new ProgressStream(body, length, prog);
        }

        public FileInfo GetCacheFileName(MediaType acceptType)
        {
            FileInfo cacheFile = null;
            var cacheFileName = CacheFileName;
            if (cacheFileName != null)
            {
                if (acceptType?.PrimaryExtension != null)
                {
                    var expectedExt = "." + acceptType.PrimaryExtension;
                    if (Path.GetExtension(cacheFileName) != expectedExt)
                    {
                        cacheFileName += expectedExt;
                    }
                }

                cacheFile = new FileInfo(cacheFileName);
            }

            return cacheFile;
        }

        private Task<Stream> OpenCachedStream(MediaType acceptType, Func<MediaType, Task<HttpWebResponse>> action, IProgress prog)
        {
            return OpenCachedStream(acceptType, (media, p) =>
            {
                p.Report(0);
                var response = action(media);
                p.Report(1);
                return response;
            }, prog);
        }

        public async Task<HttpWebResponse> Post(MediaType acceptType, IProgress prog)
        {
            var request = await CreateRequest(acceptType);
            return await request.Post(GetBodyInfo, WriteBody, prog);
        }

        public Task<HttpWebResponse> Post(MediaType acceptType)
        {
            return Post(acceptType, null);
        }

        public Task<HttpWebResponse> Post(IProgress prog)
        {
            return Post(null, prog);
        }

        public Task<HttpWebResponse> Post()
        {
            return Post(null, null);
        }

        public Task<Stream> PostForStream(MediaType acceptType, IProgress prog)
        {
            return OpenCachedStream(acceptType, new Func<MediaType, IProgress, Task<HttpWebResponse>>(Post), prog);
        }

        public Task<Stream> PostForStream(MediaType acceptType)
        {
            return OpenCachedStream(acceptType, new Func<MediaType, Task<HttpWebResponse>>(Post), null);
        }

        public Task<Stream> PostForStream(IProgress prog)
        {
            return PostForStream(null, prog);
        }

        public Task<Stream> PostForStream()
        {
            return PostForStream(null, null);
        }

        public async Task<Dictionary<string, string>> PostForHeaders(IProgress prog)
        {
            var dict = new Dictionary<string, string>();
            using (var response = await Post(null, prog))
            {
                var headers = response.Headers;
                foreach (var key in headers.AllKeys)
                {
                    dict[key] = headers[key];
                }
            }

            return dict;
        }

        public Task<Dictionary<string, string>> PostForHeaders()
        {
            return PostForHeaders(null);
        }

        public async Task<T> PostForDecoded<T>(IDeserializer<T> deserializer, IProgress prog)
        {
            var split = prog.Split("Request", "Decode");
            using (var stream = await PostForStream(deserializer.ContentType, split[0]))
            {
                return deserializer.Deserialize(stream, split[1]);
            }
        }

        public Task<T> PostForDecoded<T>(IDeserializer<T> deserializer)
        {
            return PostForDecoded(deserializer, null);
        }

        public async Task<HttpWebResponse> Get(MediaType acceptType)
        {
            var request = await CreateRequest(acceptType);
            return await request.Get();
        }

        public Task<HttpWebResponse> Get()
        {
            return Get(null);
        }

        public Task<Stream> GetStream(MediaType acceptType, IProgress prog)
        {
            return OpenCachedStream(acceptType, Get, prog);
        }

        public Task<Stream> GetStream(MediaType acceptType)
        {
            return GetStream(acceptType, null);
        }

        public Task<Stream> GetStream(IProgress prog)
        {
            return GetStream(null, prog);
        }

        public Task<Stream> GetStream()
        {
            return GetStream(null, null);
        }

        public async Task<T> GetDecoded<T>(IDeserializer<T> deserializer, IProgress prog)
        {
            using (var stream = await GetStream(deserializer.ContentType, prog))
            {
                return deserializer.Deserialize(stream);
            }
        }

        public Task<T> GetDecoded<T>(IDeserializer<T> deserializer)
        {
            return GetDecoded(deserializer, null);
        }

        public async Task Proxy(HttpListenerResponse outResponse, MediaType acceptType)
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
                using (var inResponse = await Get(acceptType))
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

        public Task Proxy(HttpListenerResponse outResponse)
        {
            return Proxy(outResponse, null);
        }
    }
}