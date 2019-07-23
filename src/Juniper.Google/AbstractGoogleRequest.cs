using System;
using Juniper.HTTP.REST;

namespace Juniper.Google
{
    public abstract class AbstractGoogleRequest<ResultType> : AbstractSingleRequest<ResultType>
    {
        private readonly bool signRequests;

        protected AbstractGoogleRequest(Uri baseServiceURI, string path, string cacheLocString, string acceptType, string extension, bool signRequests)
            : base(baseServiceURI, path, cacheLocString, acceptType, extension)
        {
            this.signRequests = signRequests;
        }

        protected override Uri MakeAuthenticatedURI(AbstractEndpoint api)
        {
            var uri = base.MakeAuthenticatedURI(api);
            if (api is Maps.Endpoint google)
            {
                uri = google.AddKey(uri);
                if (signRequests)
                {
                    uri = google.AddSignature(uri);
                }
            }
            return uri;
        }
    }
}