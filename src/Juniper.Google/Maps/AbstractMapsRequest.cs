using System;

using Juniper.HTTP.REST;
using Juniper.Serialization;

namespace Juniper.Google.Maps
{
    public abstract class AbstractMapsRequest<ResultType> : AbstractGoogleRequest<ResultType>
    {
        private static readonly Uri baseServiceURI = new Uri("https://maps.googleapis.com/maps/api/");

        protected AbstractMapsRequest(AbstractEndpoint api, IDeserializer<ResultType> deserializer, string path, bool signRequests)
            : base(api, baseServiceURI, deserializer, path, signRequests) { }
    }
}