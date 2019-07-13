using Juniper.World.GIS;
using System;

namespace Juniper.World.Imaging.GoogleMaps
{
    public abstract class Search
    {
        protected const string baseServiceURI = "https://maps.googleapis.com/maps/api/streetview";

        protected readonly UriBuilder uriBuilder;
        internal readonly string locString;
        internal readonly string extension;

        private Search(string locString, Uri baseUri, string extension)
        {
            this.locString = locString;
            uriBuilder = new UriBuilder(baseUri);
            uriBuilder.AddQuery(locString);
            this.extension = extension;
        }

        protected Search(Uri baseUri, string extension, PanoID pano)
            : this($"pano={pano}", baseUri, extension)
        {
        }

        protected Search(Uri baseUri, string extension, string placeName)
            : this($"location={placeName}", baseUri, extension)
        {
        }

        protected Search(Uri baseUri, string extension, LatLngPoint location)
            : this($"location={location.Latitude},{location.Longitude}", baseUri, extension)
        {
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