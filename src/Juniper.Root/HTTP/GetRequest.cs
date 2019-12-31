using System;
using System.IO;
using System.Linq;

using Juniper.HTTP.Client.REST;

namespace Juniper.HTTP
{
    public class GetRequest : AbstractRequest<MediaType>
    {
        public GetRequest(Uri url, MediaType contentType)
            : base(HttpMethods.GET, url, contentType)
        { }

        public override string CacheID
        {
            get
            {
                var parts = BaseURI
                    .PathAndQuery
                    .SplitX('/');

                if (parts.Length > 0)
                {
                    var last = parts.Length - 1;
                    foreach (var c in Path.GetInvalidFileNameChars())
                    {
                        parts[last] = parts[last].Replace(c, '_');
                    }
                }

                return Path.Combine(parts
                    .Prepend(BaseURI.Host)
                    .ToArray());
            }
        }
    }
}
