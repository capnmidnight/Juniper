using System;
using Juniper.HTTP.REST;

namespace Juniper.Google
{
    public abstract class AbstractGoogleRequest<ResultType> : AbstractSingleRequest<ResultType>
    {
        private bool signRequests;

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
                uri = google.AddCredentials(uri, signRequests);
            }
            return uri;
        }
    }
}
