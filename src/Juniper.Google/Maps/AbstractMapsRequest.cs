using System;

using Juniper.Serialization;

namespace Juniper.Google.Maps
{
    public abstract class AbstractMapsRequest<ResultType> : AbstractGoogleMapsRequest<ResultType>
    {
        private static readonly Uri baseServiceURI = new Uri("https://maps.googleapis.com/maps/api/");

        protected AbstractMapsRequest(GoogleMapsRequestConfiguration api, IDeserializer<ResultType> deserializer, string path, string cacheSubDirectoryName, bool signRequests)
            : base(api, baseServiceURI, deserializer, path, cacheSubDirectoryName, signRequests) { }
    }
}