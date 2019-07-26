using System;

namespace Juniper.Google.Maps
{
    public abstract class AbstractMapsRequest<ResultType> : AbstractGoogleRequest<ResultType>
    {
        private static readonly Uri baseServiceURI = new Uri("https://maps.googleapis.com/maps/api/");

        protected AbstractMapsRequest(string path, string cacheLocString, bool signRequests)
            : base(baseServiceURI, path, cacheLocString, signRequests) { }
    }
}