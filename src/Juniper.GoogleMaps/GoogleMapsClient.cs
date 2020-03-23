using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Juniper.Imaging;
using Juniper.IO;
using Juniper.Units;
using Juniper.World.GIS.Google.Geocoding;
using Juniper.World.GIS.Google.StreetView;

namespace Juniper.World.GIS.Google
{
    public class GoogleMapsClient<MetadataTypeT>
        where MetadataTypeT : MetadataResponse
    {
        private readonly IJsonFactory<GeocodingResponse> geocodingDecoder;
        private readonly IJsonFactory<MetadataTypeT> metadataDecoder;
        private readonly Dictionary<string, MetadataTypeT> metadataCache = new Dictionary<string, MetadataTypeT>();
        private readonly List<string> knownImages = new List<string>();

        private readonly string apiKey;
        private readonly string signingKey;
        private readonly CachingStrategy cache;

        private Exception lastError;

        public GoogleMapsClient(string apiKey, string signingKey, IJsonFactory<MetadataTypeT> metadataDecoder, IJsonFactory<GeocodingResponse> geocodingDecoder, CachingStrategy cache)
        {
            this.apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            this.signingKey = signingKey ?? throw new ArgumentNullException(nameof(signingKey));
            this.metadataDecoder = metadataDecoder ?? throw new ArgumentNullException(nameof(metadataDecoder));
            this.geocodingDecoder = geocodingDecoder ?? throw new ArgumentNullException(nameof(geocodingDecoder));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));


            foreach (var (fileRef, metadata) in cache.Get(metadataDecoder))
            {
                if (metadata.Location != null)
                {
                    _ = Cache(metadata);
                    var imageRef = metadata.Pano_ID + MediaType.Image.Jpeg;
                    if (cache.IsCached(imageRef))
                    {
                        knownImages.Add(metadata.Pano_ID);
                    }
                }
                else
                {
                    _ = cache.Delete(fileRef);
                }
            }

            foreach (var fileRef in cache.GetContentReferences(MediaType.Image.Jpeg))
            {
                knownImages.Add(fileRef.CacheID);
            }

            knownImages.Sort();
        }

        public string Status => lastError?.Message ?? "NONE";

        public void ClearError()
        {
            lastError = null;
        }

        public IEnumerable<(ContentReference fileRef, MetadataTypeT metadata)> CachedMetadataFiles =>
            cache.Get(metadataDecoder);

        public IReadOnlyCollection<MetadataTypeT> CachedMetadata =>
            metadataCache.Values.ToArray();

        public bool IsMetadataCached(string pano)
        {
            return metadataCache.ContainsKey(pano);
        }

        public bool IsImageCached(string pano)
        {
            return knownImages.BinarySearch(pano) >= 0;
        }

        private async Task<T> LoadAsync<T>(IDeserializer<T> deserializer, ContentReference fileRef, IProgress prog)
        {
            var value = await cache
                .LoadAsync(deserializer, fileRef, prog)
                .ConfigureAwait(false);
            if (value is MetadataTypeT metadata)
            {
                if (metadata.Status != System.Net.HttpStatusCode.OK
                    || string.IsNullOrEmpty(metadata.Pano_ID)
                    || metadata.Location is null)
                {
                    if (cache.IsCached(fileRef))
                    {
                        _ = cache.Delete(fileRef);
                    }

                    value = default;
                }
                else
                {
                    var metadataRef = new ContentReference(metadata.Pano_ID, MediaType.Application.Json);
                    if (!cache.IsCached(metadataRef))
                    {
                        await cache
                            .CopyToAsync(fileRef, cache, metadataRef)
                            .ConfigureAwait(false);
                    }
                }
            }

            return value;
        }

        public Task<GeocodingResponse> ReverseGeocodeAsync(LatLngPoint latLng, IProgress prog = null)
        {
            return LoadAsync(geocodingDecoder, new ReverseGeocodingRequest(apiKey)
            {
                Location = latLng
            }, prog);
        }

        public async Task<MetadataTypeT> GetMetadataAsync(string pano, int searchRadius = 50, IProgress prog = null)
        {
            return Cache(await LoadAsync(metadataDecoder, new MetadataRequest(apiKey, signingKey)
            {
                Pano = pano,
                Radius = searchRadius
            }, prog).ConfigureAwait(false));
        }

        public async Task<MetadataTypeT> SearchMetadataAsync(string placeName, int searchRadius = 50, IProgress prog = null)
        {
            return Cache(await LoadAsync(metadataDecoder, new MetadataRequest(apiKey, signingKey)
            {
                Place = placeName,
                Radius = searchRadius
            }, prog).ConfigureAwait(false));
        }

        public async Task<MetadataTypeT> GetMetadataAsync(LatLngPoint latLng, int searchRadius = 50, IProgress prog = null)
        {
            return Cache(await LoadAsync(metadataDecoder, new MetadataRequest(apiKey, signingKey)
            {
                Location = latLng,
                Radius = searchRadius
            }, prog).ConfigureAwait(false));
        }

        public async Task<MetadataTypeT> SearchMetadataAsync(string searchLocation, string searchPano, LatLngPoint searchPoint, int searchRadius, IProgress prog = null)
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

        private MetadataTypeT Cache(MetadataTypeT metadata)
        {
            if (metadata != null)
            {
                metadataCache[metadata.Location.ToString(CultureInfo.InvariantCulture)] = metadata;
                metadataCache[metadata.Pano_ID] = metadata;
            }

            return metadata;
        }

        public async Task<MetadataTypeT> FindClosestMetadataAsync(LatLngPoint point, int searchRadius)
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

        public async Task<Stream> GetImageAsync(string pano, int fov, int heading, int pitch, IProgress prog = null)
        {
            var imageStream = await cache.GetStreamAsync(new ImageRequest(apiKey, signingKey, new Size(640, 640))
            {
                Pano = pano,
                FOV = fov,
                Heading = heading,
                Pitch = pitch
            }, prog).ConfigureAwait(false);

            if (imageStream is object
                && knownImages.MaybeAdd(pano))
            {
                knownImages.Sort();
            }

            return imageStream;
        }
    }
}
