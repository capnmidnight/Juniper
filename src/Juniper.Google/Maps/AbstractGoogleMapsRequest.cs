using System;

using Juniper.HTTP.REST;
using Juniper.Serialization;

namespace Juniper.Google.Maps
{
    public abstract class AbstractGoogleMapsRequest<ResultType> : AbstractSingleRequest<ResultType>
    {
        private readonly bool signRequests;
        private readonly GoogleMapsRequestConfiguration google;

        protected AbstractGoogleMapsRequest(GoogleMapsRequestConfiguration api, Uri baseServiceURI, IDeserializer<ResultType> deserializer, string path, string cacheSubDirectoryName, bool signRequests)
            : base(api, baseServiceURI, deserializer, path, cacheSubDirectoryName)
        {
            google = api;
            this.signRequests = signRequests;
        }

        protected override Uri AuthenticatedURI
        {
            get
            {
                var uri = base.AuthenticatedURI;
                uri = google.AddKey(uri);
                if (signRequests)
                {
                    uri = google.AddSignature(uri);
                }
                return uri;
            }
        }
    }
}