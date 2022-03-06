using Juniper.IO;
using Juniper.Progress;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.HTTP.REST
{
    public abstract class AbstractRequest<MediaTypeT> : AbstractStreamSource
        where MediaTypeT : MediaType
    {
        protected static Uri AddPath(Uri baseURI, string path)
        {
            var uriBuilder = new UriBuilder(baseURI);
            uriBuilder.Path = Path.Combine(uriBuilder.Path, path);
            return uriBuilder.Uri;
        }

        private readonly HttpClient http;
        private readonly HttpMethod method;
        private readonly Uri serviceURI;

        private readonly IDictionary<string, List<string>> queryParams =
            new SortedDictionary<string, List<string>>();

        protected AbstractRequest(HttpClient http, HttpMethod method, Uri serviceURI, MediaTypeT contentType)
            : base(contentType)
        {
            this.http = http;
            this.method = method;
            this.serviceURI = serviceURI;

            ResponseBodyMediaType = contentType;
        }

        public MediaTypeT ResponseBodyMediaType
        {
            get;
        }

        public override MediaType ContentType => ResponseBodyMediaType;

        public override bool Equals(object obj)
        {
            return obj is not null
                && obj is AbstractRequest<MediaTypeT> req
                && req.CacheID == CacheID;
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

        protected virtual Uri AuthenticatedURI => BaseURI;

        protected string StandardRequestCacheID
        {
            get
            {
                var invalid = Path.GetInvalidFileNameChars();
                var parts = (BaseURI.Host + BaseURI.PathAndQuery)
                    .SplitX('/');

                var sb = new StringBuilder();

                for (var i = 0; i < parts.Length; ++i)
                {
                    var part = parts[i];
                    for (var j = 0; j < part.Length; ++j)
                    {
                        sb.Append(invalid.Contains(part[j])
                            ? '_'
                            : part[j]);
                    }

                    if (i < parts.Length - 1)
                    {
                        sb.Append(Path.DirectorySeparatorChar);
                    }
                }

                return sb.ToString();
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

        protected virtual void ModifyRequest(HttpRequestMessage request, IProgress prog = null) { }

        public override async Task<Stream> GetStreamAsync(IProgress prog = null)
        {
            var request = new HttpRequestMessage(method, AuthenticatedURI);

            if (ResponseBodyMediaType is not null)
            {
                request.Accept(ResponseBodyMediaType);
            }

            var progs = prog.Split("Requesting", "Retrieving");

            ModifyRequest(request, progs[0]);

            var response = await http.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            return new ProgressStream(stream, stream.Length, progs[1], true);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                base.GetHashCode(),
                method,
                queryParams,
                ContentType,
                CacheID,
                BaseURI);
        }
    }
}