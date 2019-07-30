using System;
using Juniper.HTTP.REST;
using Juniper.Serialization;

namespace Juniper.Google
{
    public abstract class AbstractGoogleRequest<ResultType> : AbstractSingleRequest<ResultType>
    {
        private readonly bool signRequests;

        protected AbstractGoogleRequest(Uri baseServiceURI, IDeserializer<ResultType> deserializer, string path, string cacheLocString, bool signRequests)
            : base(baseServiceURI, deserializer, path, cacheLocString)
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