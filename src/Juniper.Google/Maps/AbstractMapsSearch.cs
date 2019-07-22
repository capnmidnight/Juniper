using System;

namespace Juniper.Google.Maps
{
    public abstract class AbstractMapsSearch<ResultType> : AbstractGoogleRequest<ResultType>
    {
        private static readonly Uri baseServiceURI = new Uri("https://maps.googleapis.com/maps/api/");

        protected AbstractMapsSearch(string path, string cacheLocString, string acceptType, string extension, bool signRequests)
            : base(baseServiceURI, path, cacheLocString, acceptType, extension, signRequests) { }
    }
}