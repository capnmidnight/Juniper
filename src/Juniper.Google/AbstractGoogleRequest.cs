using System;

using Juniper.HTTP.REST;
using Juniper.Serialization;

namespace Juniper.Google
{
    public abstract class AbstractGoogleRequest<ResultType> : AbstractSingleRequest<ResultType>
    {
        private readonly bool signRequests;

        protected AbstractGoogleRequest(AbstractRequestConfiguration api, Uri baseServiceURI, IDeserializer<ResultType> deserializer, string path, string cacheSubDirectoryName, bool signRequests)
            : base(api, baseServiceURI, deserializer, path, cacheSubDirectoryName)
        {
            this.signRequests = signRequests;
        }

        protected override Uri AuthenticatedURI
        {
            get
            {
                var uri = base.AuthenticatedURI;
                if (api is Maps.GoogleMapsRequestConfiguration google)
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
}