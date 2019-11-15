using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.Imaging;
using Juniper.IO;
using Juniper.Progress;
using Juniper.World.GIS.Google.Geocoding;
using Juniper.World.GIS.Google.StreetView;

namespace Juniper.World.GIS.Google
{
    public class GoogleMapsClient
    {
        private readonly IJsonDecoder<GeocodingResponse> geocodingDecoder;
        private readonly IJsonDecoder<MetadataResponse> metadataDecoder;

        private readonly string apiKey;
        private readonly string signingKey;
        private readonly CachingStrategy cache;

        private Exception lastError;

        public GoogleMapsClient(string apiKey, string signingKey, IJsonDecoder<MetadataResponse> metadataDecoder, IJsonDecoder<GeocodingResponse> geocodingDecoder, CachingStrategy cache)
        {
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

        private async Task<T> Load<T>(IDeserializer<T> deserializer, ContentReference fileRef, IProgress prog)
        {
            var value = await cache.Load(deserializer, fileRef, prog);
            if(value is MetadataResponse metadata)
            {
                if (metadata.status != System.Net.HttpStatusCode.OK
                    || string.IsNullOrEmpty(metadata.pano_id)
                    || metadata.location is null)
                {
                    if (cache.IsCached(fileRef))
                    {
                        cache.Delete(fileRef);
                    }

                    value = default;
                }
                else
                {
                    var metadataRef = new ContentReference(metadata.pano_id, MediaType.Application.Json);
                    await cache.CopyTo(fileRef, cache, metadataRef);
                }
            }
            return value;
        }

        public Task<GeocodingResponse> ReverseGeocode(LatLngPoint latLng, IProgress prog = null)
        {
            return Load(geocodingDecoder, new ReverseGeocodingRequest(apiKey)
            {
                Location = latLng
            }, prog);
        }

        public Task<MetadataResponse> GetMetadata(string pano, int searchRadius = 50, IProgress prog = null)
        {
            return Load(metadataDecoder, new MetadataRequest(apiKey, signingKey)
            {
                Pano = pano,
                Radius = searchRadius
            }, prog);
        }

        public Task<MetadataResponse> SearchMetadata(string placeName, int searchRadius = 50, IProgress prog = null)
        {
            return Load(metadataDecoder, new MetadataRequest(apiKey, signingKey)
            {
                Place = placeName,
                Radius = searchRadius
            }, prog);
        }

        public Task<MetadataResponse> GetMetadata(LatLngPoint latLng, int searchRadius = 50, IProgress prog = null)
        {
            return Load(metadataDecoder, new MetadataRequest(apiKey, signingKey)
            {
                Location = latLng,
                Radius = searchRadius
            }, prog);
        }

        public Task<Stream> GetImage(string pano, int fov, int heading, int pitch, IProgress prog = null)
        {
            return cache.Open(new ImageRequest(apiKey, signingKey, new Size(640, 640))
            {
                Pano = pano,
                FOV = fov,
                Heading = heading,
                Pitch = pitch
            }, prog);
        }
    }
}
