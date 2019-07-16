using System;

namespace Juniper.Google.Maps
{
    public abstract class AbstractMapsSearch
    {
        private const string baseServiceURI = "https://maps.googleapis.com/maps/api/";

        protected readonly UriBuilder uriBuilder;
        internal readonly string extension;

        protected AbstractMapsSearch(string path, string extension)
        {
            uriBuilder = new UriBuilder(baseServiceURI);
            uriBuilder.Path += path;
            this.extension = extension;
        }
        public Uri Uri
        {
            get
            {
                return uriBuilder.Uri;
            }
        }
    }
}