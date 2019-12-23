using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    public class GetRequest : REST.AbstractRequest<MediaType>
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
