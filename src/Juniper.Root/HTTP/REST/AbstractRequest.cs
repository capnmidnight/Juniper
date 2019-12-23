using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.IO;
using Juniper.Progress;

namespace Juniper.HTTP.REST
{
    public abstract class AbstractRequest<MediaTypeT> : StreamSource
        where MediaTypeT : MediaType
    {
        protected static Uri AddPath(Uri baseURI, string path)
        {
            var uriBuilder = new UriBuilder(baseURI);
            uriBuilder.Path = Path.Combine(uriBuilder.Path, path);
            return uriBuilder.Uri;
        }

        private readonly HttpMethod method;
        private readonly Uri serviceURI;

        private readonly IDictionary<string, List<string>> queryParams =
            new SortedDictionary<string, List<string>>();

        protected AbstractRequest(HttpMethod method, Uri serviceURI, MediaTypeT contentType)
            : base(contentType)
        {
            this.method = method;
            this.serviceURI = serviceURI;
            MediaType = contentType;
        }

        public MediaTypeT MediaType
        {
            get;
        }

        public override MediaType ContentType
        {
            get
            {
                return MediaType;
            }
        }

        public override int GetHashCode()
        {
            return CacheID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null
                && obj is AbstractRequest<MediaTypeT> req
                && req.CacheID == CacheID;
        }

        public override string CacheID
        {
            get
            {
                return PathExt.FixPath(BaseURI.PathAndQuery.Substring(1));
            }
        }

        protected virtual Uri BaseURI
        {
            get
            {
                var uriBuilder = new UriBuilder(serviceURI)
                {
                    Query = queryParams.ToString("=", "&")
                };
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

        protected virtual void ModifyRequest(HttpWebRequest request) { }

        protected virtual BodyInfo GetBodyInfo()
        {
            return null;
        }

        protected virtual void WriteBody(Stream stream) { }

        public async Task<HttpWebResponse> GetResponseAsync(IProgress prog = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(AuthenticatedURI);

            request = request.Method(method);

            if (AuthenticatedURI.Scheme == "http")
            {
                request.Header("Upgrade-Insecure-Requests", 1);
            }

            if (MediaType != null)
            {
                request.Accept = MediaType;
            }

            ModifyRequest(request);

            var info = GetBodyInfo();
            if (info == null)
            {
                request.ContentLength = 0;
            }
            else
            {
                request.ContentLength = info.Length;
                request.ContentType = info.MIMEType;
            }

            if (request.ContentLength > 0)
            {
                using (var stream = new ProgressStream(await request
                    .GetRequestStreamAsync()
                    .ConfigureAwait(false), request.ContentLength, prog))
                {
                    WriteBody(stream);
                    stream.Flush();
                }
            }

            return (HttpWebResponse)await request
                .GetResponseAsync()
                .ConfigureAwait(false);
        }

        public override async Task<Stream> GetStream(IProgress prog = null)
        {
            var progs = prog.Split("Get", "Read");
            prog = progs[1];
            var response = await GetResponseAsync(progs[0]).ConfigureAwait(false);
            var stream = response.GetResponseStream();
            if (prog != null)
            {
                var length = response.ContentLength;
                stream = new ProgressStream(stream, length, prog);
            }
            return stream;
        }
    }
}