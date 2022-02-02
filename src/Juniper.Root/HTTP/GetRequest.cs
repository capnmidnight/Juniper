using Juniper.HTTP.REST;

using System;
using System.Net.Http;

namespace Juniper.HTTP
{
    public class GetRequest : AbstractRequest<MediaType>
    {
        public GetRequest(Uri url, MediaType contentType)
            : base(HttpMethod.Get, url, contentType, false)
        { }

        protected override string InternalCacheID =>
            StandardRequestCacheID;
    }
}
