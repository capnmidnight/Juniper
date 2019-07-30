using System;
using Juniper.Serialization;

namespace Juniper.Google.Maps
{
    public abstract class AbstractMapsRequest<ResultType> : AbstractGoogleRequest<ResultType>
    {
        private static readonly Uri baseServiceURI = new Uri("https://maps.googleapis.com/maps/api/");

        protected AbstractMapsRequest(IDeserializer<ResultType> deserializer, string path, string cacheLocString, bool signRequests)
            : base(baseServiceURI, deserializer, path, cacheLocString, signRequests) { }
    }
}