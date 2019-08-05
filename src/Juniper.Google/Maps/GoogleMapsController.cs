using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Juniper.Google.Maps.StreetView;
using Juniper.Image;
using Juniper.Progress;
using Juniper.World.GIS;

namespace Juniper.Google.Maps
{
    public class GoogleMapsController
    {
        public static GoogleMapsController CreateController(Stream config, Size size, DirectoryInfo cacheLocation = null)
        {
            using (var reader = new StreamReader(config))
            {
                var apiKey = reader.ReadLine();
                var signingKey = reader.ReadLine();
                return new GoogleMapsController(apiKey, signingKey, size, cacheLocation);
            }
        }

        public static GoogleMapsController CreateController(FileInfo configFile, Size size, DirectoryInfo cacheLocation = null)
        {
            using (var stream = configFile.OpenRead())
            {
                return CreateController(stream, size, cacheLocation);
            }
        }

        public static GoogleMapsController CreateController(string configFileName, Size size, DirectoryInfo cacheLocation = null)
        {
            return CreateController(new FileInfo(configFileName), size, cacheLocation);
        }

        private readonly Dictionary<string, Task<MetadataResponse>> metadataRequestCache = new Dictionary<string, Task<MetadataResponse>>();
        private readonly Dictionary<string, Task<ImageData>> imageRequestCache = new Dictionary<string, Task<ImageData>>();

        private readonly GoogleMapsRequestConfiguration gmaps;
        private readonly Size imageSize;

        public GoogleMapsController(string apiKey, string signingKey, Size imageSize, DirectoryInfo cacheLocation = null)
        {
            gmaps = new GoogleMapsRequestConfiguration(apiKey, signingKey, cacheLocation);
            this.imageSize = imageSize;
        }

        private static Task<T> GetCachedResponse<T>(
            AbstractGoogleMapsRequest<T> request,
            Dictionary<string, Task<T>> requestCache,
            Func<Task<T>> createRequest)
        {
            var key = request.CacheID;
            Task<T> iter;
            lock (requestCache)
            {
                if (!requestCache.ContainsKey(key))
                {
                    requestCache[key] = createRequest();
                }

                iter = requestCache[key];
            }

            return iter;
        }

        private Task<MetadataResponse> GetMetadata(MetadataRequest request, IProgress prog)
        {
            return GetCachedResponse(
                request,
                metadataRequestCache,
                () => request.Get(prog));
        }

        public Task<MetadataResponse> GetMetadata(LatLngPoint position, IProgress prog = null)
        {
            var metadataRequest = new MetadataRequest(gmaps, position);
            return GetMetadata(metadataRequest, prog);
        }

        public Task<MetadataResponse> GetMetadata(PlaceName position, IProgress prog = null)
        {
            var metadataRequest = new MetadataRequest(gmaps, position);
            return GetMetadata(metadataRequest, prog);
        }

        public Task<ImageData> GetImage(PanoID pano, IProgress prog = null)
        {
            var imageRequest = new CrossCubeMapRequest(gmaps, pano, imageSize);
            return GetCachedResponse(
                imageRequest,
                imageRequestCache,
                () => imageRequest.GetJPEG(prog));
        }
    }
}