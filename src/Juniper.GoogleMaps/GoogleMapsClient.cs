using Juniper.IO;
using Juniper.Progress;
using Juniper.Units;
using Juniper.World.GIS.Google.Geocoding;
using Juniper.World.GIS.Google.StreetView;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Juniper.World.GIS.Google
{
    public class GoogleMapsClient : GoogleMapsStreamingClient, IGoogleMapsClient
    {
        private readonly IJsonFactory<GeocodingResponse> geocodingDecoder;
        private readonly IJsonFactory<MetadataResponse> metadataDecoder;
        private readonly Dictionary<string, MetadataResponse> metadataCache = new();
        private readonly List<string> knownImages = new();

        public GoogleMapsClient(HttpClient http, string apiKey, string signingKey, IJsonFactory<MetadataResponse> metadataDecoder, IJsonFactory<GeocodingResponse> geocodingDecoder, CachingStrategy cache)
            : base(http, apiKey, signingKey, cache)
        {
            this.metadataDecoder = metadataDecoder ?? throw new ArgumentNullException(nameof(metadataDecoder));
            this.geocodingDecoder = geocodingDecoder ?? throw new ArgumentNullException(nameof(geocodingDecoder));


            foreach ((var fileRef, var metadata) in cache.Get(metadataDecoder))
            {
                if (metadata.Location != null)
                {
                    _ = Encache(metadata);
                    var imageRef = metadata.Pano_id + MediaType.Image_Jpeg;
                    if (cache.IsCached(imageRef))
                    {
                        knownImages.Add(metadata.Pano_id);
                    }
                }
                else
                {
                    _ = cache.Delete(fileRef);
                }
            }

            foreach (var fileRef in cache.GetContentReferences(MediaType.Image_Jpeg))
            {
                knownImages.Add(fileRef.CacheID);
            }

            knownImages.Sort();
        }

        public GoogleMapsClient(HttpClient http, string apiKey, string signingKey, IJsonFactory<MetadataResponse> metadataFactory, IJsonFactory<GeocodingResponse> geocodingFactory)
            : this(http, apiKey, signingKey, metadataFactory, geocodingFactory, CachingStrategy.GetTempCache())
        { }


        public IEnumerable<(ContentReference fileRef, MetadataResponse metadata)> CachedMetadataFiles =>
            Cache.Get(metadataDecoder);

        public IReadOnlyCollection<MetadataResponse> CachedMetadata =>
            metadataCache.Values.ToArray();

        public bool IsMetadataCached(string pano)
        {
            return metadataCache.ContainsKey(pano);
        }

        public bool IsImageCached(string pano)
        {
            return knownImages.BinarySearch(pano) >= 0;
        }

        private async Task<T> LoadAsync<T, M>(IDeserializer<T, M> deserializer, ContentReference fileRef, IProgress prog)
            where M : MediaType
        {
            var value = await Cache
                .LoadAsync(deserializer, fileRef, prog)
                .ConfigureAwait(false);
            if (value is MetadataResponse metadata
                && metadata.Status == System.Net.HttpStatusCode.OK
                    && !string.IsNullOrEmpty(metadata.Pano_id)
                    && metadata.Location is not null)
            {
                var metadataRef = new ContentReference(metadata.Pano_id, MediaType.Application_Json);
                if (!Cache.IsCached(metadataRef))
                {
                    await Cache
                        .CopyToAsync(fileRef, Cache, metadataRef)
                        .ConfigureAwait(false);
                }
            }

            return value;
        }

        public Task<GeocodingResponse> ReverseGeocodeAsync(LatLngPoint latLng, IProgress prog = null)
        {
            return LoadAsync(geocodingDecoder, new ReverseGeocodingRequest(Http, ApiKey)
            {
                Location = latLng
            }, prog);
        }

        public async Task<MetadataResponse> GetMetadataAsync(string pano, int searchRadius = 50, IProgress prog = null)
        {
            return Encache(await LoadAsync(metadataDecoder, new MetadataRequest(Http, ApiKey, SigningKey)
            {
                Pano = pano,
                Radius = searchRadius
            }, prog).ConfigureAwait(false));
        }

        public async Task<MetadataResponse> SearchMetadataAsync(string placeName, int searchRadius = 50, IProgress prog = null)
        {
            return Encache(await LoadAsync(metadataDecoder, new MetadataRequest(Http, ApiKey, SigningKey)
            {
                Place = placeName,
                Radius = searchRadius
            }, prog).ConfigureAwait(false));
        }

        public async Task<MetadataResponse> GetMetadataAsync(LatLngPoint latLng, int searchRadius = 50, IProgress prog = null)
        {
            return Encache(await LoadAsync(metadataDecoder, new MetadataRequest(Http, ApiKey, SigningKey)
            {
                Location = latLng,
                Radius = searchRadius
            }, prog).ConfigureAwait(false));
        }

        public async Task<MetadataResponse> SearchMetadataAsync(string searchLocation, string searchPano, LatLngPoint searchPoint, int searchRadius, IProgress prog = null)
        {
            try
            {
                if (metadataCache.ContainsKey(searchLocation))
                {
                    return metadataCache[searchLocation];
                }

                var metaSubProgs = prog.Split("Searching by Pano_ID", "Searching by Lat/Lng", "Searching by Location Name");
                if (searchPano != null)
                {
                    return await GetMetadataAsync(searchPano, searchRadius, metaSubProgs[0])
                        .ConfigureAwait(false);
                }

                if (searchPoint != null)
                {
                    return await GetMetadataAsync(searchPoint, searchRadius, metaSubProgs[1])
                        .ConfigureAwait(false);
                }

                if (searchLocation != null)
                {
                    return await SearchMetadataAsync(searchLocation, searchRadius, metaSubProgs[2])
                        .ConfigureAwait(false);
                }

                return default;
            }
            finally
            {
                prog.Report(1);
            }
        }

        private MetadataResponse Encache(MetadataResponse metadata)
        {
            if (metadata != null
                && metadata.Location is not null
                && metadata.Pano_id is not null)
            {
                metadataCache[metadata.Location.ToString(CultureInfo.InvariantCulture)] = metadata;
                metadataCache[metadata.Pano_id] = metadata;
            }

            return metadata;
        }

        public async Task<MetadataResponse> FindClosestMetadataAsync(LatLngPoint point, int searchRadius)
        {
            var closestMetadata = await GetMetadataAsync(point, searchRadius, null)
                .ConfigureAwait(false);

            var minDistance = closestMetadata?.Location.Distance(point)
                ?? float.MaxValue;
            foreach (var metadata in metadataCache.Values)
            {
                var distance = point.Distance(metadata.Location);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestMetadata = metadata;
                }
            }

            return closestMetadata;
        }

        public override Task<Stream> GetImageStreamAsync(string pano, int fovDegrees, int headingDegrees, int pitchDegrees, IProgress prog = null)
        {
            var imageStream = base.GetImageStreamAsync(pano, fovDegrees, headingDegrees, pitchDegrees, prog);

            if (imageStream is not null
                && knownImages.MaybeAdd(pano))
            {
                knownImages.Sort();
            }

            return imageStream;
        }
    }
}
