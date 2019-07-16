using System;

namespace Juniper.Google.Maps
{
    public abstract class AbstractMapsSearch<T> : AbstractSingleSearch<T>
    {
        private static readonly Uri baseServiceURI = new Uri("https://maps.googleapis.com/maps/api/");

        protected AbstractMapsSearch(string path, string cacheLocString, string extension)
            : base(baseServiceURI, path, cacheLocString, extension) { }
    }
}