using System;
using System.Threading.Tasks;

using Juniper.GIS.Google.Geocoding;
using Juniper.GIS.Google.StreetView;
using Juniper.Imaging;
using Juniper.IO;
using Juniper.Progress;

namespace Juniper.World.GIS.Google
{
    public class GoogleMapsClient<T>
    {
        private readonly IImageCodec<T> imageDecoder;
        private readonly IDeserializer<GeocodingResponse> geocodingDecoder;
        private readonly IDeserializer<MetadataResponse> metadataDecoder;

        private readonly string apiKey;
        private readonly string signingKey;
        private readonly CachingStrategy cache;

        private Exception lastError;

        public GoogleMapsClient(string apiKey, string signingKey, IImageCodec<T> imageDecoder, IDeserializer<MetadataResponse> metadataDecoder, IDeserializer<GeocodingResponse> geocodingDecoder, CachingStrategy cache)
        {
            this.imageDecoder = imageDecoder;
            this.metadataDecoder = metadataDecoder;
            this.geocodingDecoder = geocodingDecoder;
            this.apiKey = apiKey;
            this.signingKey = signingKey;
            this.cache = cache;
        }

        public string Status
        {
            get
            {
                return lastError?.Message ?? "NONE";
            }
        }

        public void ClearError()
        {
            lastError = null;
        }

        public Task<GeocodingResponse> ReverseGeocode(LatLngPoint latLng, IProgress prog = null)
        {
            var geocodingRequest = new ReverseGeocodingRequest(apiKey)
            {
                Location = latLng
            };
            return cache.Decode(geocodingRequest, geocodingDecoder);
        }

        public Task<MetadataResponse> GetMetadata(string pano, int searchRadius = 50, IProgress prog = null)
        {
            var metadataRequest = new MetadataRequest(apiKey, signingKey)
            {
                Pano = pano,
                Radius = searchRadius
            };

            return cache.Decode(metadataRequest, metadataDecoder);
        }

        public Task<MetadataResponse> SearchMetadata(string placeName, int searchRadius = 50, IProgress prog = null)
        {
            var metadataRequest = new MetadataRequest(apiKey, signingKey)
            {
                Place = placeName,
                Radius = searchRadius
            };

            return cache.Decode(metadataRequest, metadataDecoder);
        }

        public Task<MetadataResponse> GetMetadata(LatLngPoint latLng, int searchRadius = 50, IProgress prog = null)
        {
            var metadataRequest = new MetadataRequest(apiKey, signingKey)
            {
                Location = latLng,
                Radius = searchRadius
            };

            return cache.Decode(metadataRequest, metadataDecoder);
        }

        public Task<T> GetImage(string pano, int fov, int heading, int pitch, IProgress prog = null)
        {
            var imageRequest = new ImageRequest(apiKey, signingKey, new Size(640, 640))
            {
                Pano = pano,
                FOV = fov,
                Heading = heading,
                Pitch = pitch
            };

            return cache.Decode(imageRequest, imageDecoder);
        }
    }
}
