using Juniper.IO;
using Juniper.Progress;
using Juniper.World.GIS.Google.Geocoding;
using Juniper.World.GIS.Google.StreetView;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Juniper.World.GIS.Google
{
    public class GoogleMapsStreamingClient : IGoogleMapsStreamingClient
    {
        protected string ApiKey { get; private set; }
        protected string SigningKey { get; private set; }
        protected CachingStrategy Cache { get; private set; }

        private Exception lastError;

        public GoogleMapsStreamingClient(string apiKey, string signingKey, CachingStrategy cache)
        {
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            SigningKey = signingKey ?? throw new ArgumentNullException(nameof(signingKey));
            Cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public GoogleMapsStreamingClient(string apiKey, string signingKey)
            : this(apiKey, signingKey, CachingStrategy.GetTempCache())
        { }

        public string Status => lastError?.Message ?? "NONE";

        public void ClearError()
        {
            lastError = null;
        }

        public Task<Stream> ReverseGeocodeStreamAsync(LatLngPoint latLng, IProgress prog = null)
        {
            return Cache.GetStreamAsync(new ReverseGeocodingRequest(ApiKey)
            {
                Location = latLng
            }, prog);
        }

        public Task<Stream> GetMetadataStreamAsync(string pano, int searchRadius = 50, IProgress prog = null)
        {
            return Cache.GetStreamAsync(new MetadataRequest(ApiKey, SigningKey)
            {
                Pano = pano,
                Radius = searchRadius
            }, prog);
        }

        public Task<Stream> SearchMetadataStreamAsync(string placeName, int searchRadius = 50, IProgress prog = null)
        {
            return Cache.GetStreamAsync(new MetadataRequest(ApiKey, SigningKey)
            {
                Place = placeName,
                Radius = searchRadius
            }, prog);
        }

        public Task<Stream> GetMetadataStreamAsync(LatLngPoint latLng, int searchRadius = 50, IProgress prog = null)
        {
            return Cache.GetStreamAsync(new MetadataRequest(ApiKey, SigningKey)
            {
                Location = latLng,
                Radius = searchRadius
            }, prog);
        }

        public async Task<Stream> SearchMetadataStreamAsync(string searchLocation, string searchPano, LatLngPoint searchPoint, int searchRadius, IProgress prog = null)
        {
            try
            {
                var metaSubProgs = prog.Split("Searching by Pano_ID", "Searching by Lat/Lng", "Searching by Location Name");
                if (searchPano != null)
                {
                    return await GetMetadataStreamAsync(searchPano, searchRadius, metaSubProgs[0])
                        .ConfigureAwait(false);
                }

                if (searchPoint != null)
                {
                    return await GetMetadataStreamAsync(searchPoint, searchRadius, metaSubProgs[1])
                        .ConfigureAwait(false);
                }

                if (searchLocation != null)
                {
                    return await SearchMetadataStreamAsync(searchLocation, searchRadius, metaSubProgs[2])
                        .ConfigureAwait(false);
                }

                return default;
            }
            finally
            {
                prog.Report(1);
            }
        }

        public virtual Task<Stream> GetImageStreamAsync(string pano, int fov, int heading, int pitch, IProgress prog = null)
        {
            return Cache.GetStreamAsync(new ImageRequest(ApiKey, SigningKey, new Size(640, 640))
            {
                Pano = pano,
                FOV = fov,
                Heading = heading,
                Pitch = pitch
            }, prog);
        }
    }
}
