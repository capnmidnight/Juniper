using System;

using Juniper.HTTP.REST;

namespace Juniper.HTTP
{
    public class GetRequest : AbstractRequest<MediaType>
    {
        public GetRequest(Uri url, MediaType contentType)
            : base(HttpMethods.GET, url, contentType, false)
        { }

        protected override string InternalCacheID =>
            StandardRequestCacheID;
    }
}
