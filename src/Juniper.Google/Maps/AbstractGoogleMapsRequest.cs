using System;

using Juniper.HTTP.REST;
using Juniper.Serialization;

namespace Juniper.Google.Maps
{
    public abstract class AbstractGoogleMapsRequest<DecoderType, ResultType> : AbstractRequest<DecoderType, ResultType>
        where DecoderType : IDeserializer<ResultType>
    {
        private readonly bool signRequests;
        private readonly GoogleMapsRequestConfiguration google;

        protected AbstractGoogleMapsRequest(GoogleMapsRequestConfiguration api, DecoderType deserializer, string path, string cacheSubDirectoryName, bool signRequests)
            : base(api, deserializer, path, cacheSubDirectoryName)
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