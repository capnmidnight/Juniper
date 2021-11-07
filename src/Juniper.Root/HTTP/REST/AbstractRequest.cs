using Juniper.IO;
using Juniper.Progress;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        private readonly HttpMethods method;
        private readonly Uri serviceURI;
        private readonly bool hasRequestBody;

        private readonly IDictionary<string, List<string>> queryParams =
            new SortedDictionary<string, List<string>>();

        protected AbstractRequest(HttpMethods method, Uri serviceURI, MediaTypeT contentType, bool hasRequestBody)
            : base(contentType)
        {
            this.method = method;
            this.serviceURI = serviceURI;
            this.hasRequestBody = hasRequestBody;

            MediaType = contentType;
        }

        public MediaTypeT MediaType
        {
            get;
        }

        public override MediaType ContentType => MediaType;

        public override bool Equals(object obj)
        {
            return obj is object
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

        protected virtual void ModifyRequest(HttpWebRequest request) { }

        protected virtual BodyInfo GetBodyInfo()
        {
            return null;
        }

        protected virtual void WriteBody(Stream stream) { }

        public override async Task<Stream> GetStreamAsync(IProgress prog = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(AuthenticatedURI);

            request = request.Method(method);

            if (AuthenticatedURI.Scheme == "http")
            {
                request.Header("Upgrade-Insecure-Requests", 1);
            }

            if (MediaType is object)
            {
                request.Accept = MediaType;
            }

            ModifyRequest(request);

            if (hasRequestBody)
            {
                var progs = prog.Split("Requesting", "Retrieving");
                prog = progs[1];
                await request.WriteBodyAsync(GetBodyInfo, WriteBody, progs[0])
                    .ConfigureAwait(false);
            }
            else
            {
                request.ContentLength = 0;
            }

            var response = (HttpWebResponse)await request
                .GetResponseAsync()
                .ConfigureAwait(false);

            return new ProgressStream(
                response.GetResponseStream(),
                response.ContentLength,
                prog, false);
        }

        public override int GetHashCode()
        {
            var hashCode = 1106372483;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + method.GetHashCode();
            hashCode = hashCode * -1521134295 + hasRequestBody.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IDictionary<string, List<string>>>.Default.GetHashCode(queryParams);
            hashCode = hashCode * -1521134295 + EqualityComparer<MediaType>.Default.GetHashCode(ContentType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CacheID);
            hashCode = hashCode * -1521134295 + EqualityComparer<Uri>.Default.GetHashCode(BaseURI);
            return hashCode;
        }
    }
}