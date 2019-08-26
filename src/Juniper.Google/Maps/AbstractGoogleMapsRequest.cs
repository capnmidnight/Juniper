using System;

using Juniper.HTTP.REST;

namespace Juniper.Google.Maps
{
    public abstract class AbstractGoogleMapsRequest : AbstractRequest
    {
        private readonly bool signRequests;
        private readonly GoogleMapsRequestConfiguration google;

        protected AbstractGoogleMapsRequest(GoogleMapsRequestConfiguration api, string path, string cacheSubDirectoryName, bool signRequests)
            : base(api, path, cacheSubDirectoryName)
        {
            google = api;
            this.signRequests = signRequests;
        }

        protected override Uri AuthenticatedURI
        {
            get
            {
                var uri = google.AddKey(base.AuthenticatedURI);
                if (signRequests)
                {
                    uri = google.AddSignature(uri);
                }
                return uri;
            }
        }
    }
}