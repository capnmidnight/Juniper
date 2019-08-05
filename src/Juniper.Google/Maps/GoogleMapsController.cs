using System;
using System.Collections.Generic;
using System.IO;

using Juniper.Google.Maps.StreetView;
using Juniper.Image;
using Juniper.Progress;
using Juniper.World.GIS;

namespace Juniper.Google.Maps
{
    public class GoogleMapsController<TextureType>
    {
        public static GoogleMapsController<TextureType> CreateController(Stream config, Size size, Func<ImageData, IProgress, IEnumerator<TextureType>> createTexture, DirectoryInfo cacheLocation = null)
        {
            using (var reader = new StreamReader(config))
            {
                var apiKey = reader.ReadLine();
                var signingKey = reader.ReadLine();
                return new GoogleMapsController<TextureType>(apiKey, signingKey, size, createTexture, cacheLocation);
            }
        }

        public static GoogleMapsController<TextureType> CreateController(FileInfo configFile, Size size, Func<ImageData, IProgress, IEnumerator<TextureType>> createTexture, DirectoryInfo cacheLocation = null)
        {
            using (var stream = configFile.OpenRead())
            {
                return CreateController(stream, size, createTexture, cacheLocation);
            }
        }

        public static GoogleMapsController<TextureType> CreateController(string configFileName, Size size, Func<ImageData, IProgress, IEnumerator<TextureType>> createTexture, DirectoryInfo cacheLocation = null)
        {
            return CreateController(new FileInfo(configFileName), size, createTexture, cacheLocation);
        }

        private readonly Dictionary<string, IEnumerator<MetadataResponse>> metadataRequestCache = new Dictionary<string, IEnumerator<MetadataResponse>>();
        private readonly Dictionary<string, IEnumerator<TextureType>> textureRequestCache = new Dictionary<string, IEnumerator<TextureType>>();
        private readonly Func<ImageData, IProgress, IEnumerator<TextureType>> createTexture;

        private readonly GoogleMapsRequestConfiguration gmaps;
        private readonly Size imageSize;

        public GoogleMapsController(string apiKey, string signingKey, Size imageSize, Func<ImageData, IProgress, IEnumerator<TextureType>> createTexture, DirectoryInfo cacheLocation = null)
        {
            gmaps = new GoogleMapsRequestConfiguration(apiKey, signingKey, cacheLocation);
            this.imageSize = imageSize;
            this.createTexture = createTexture;
        }

        private static IEnumerator<T> GetCachedResponse<T>(
            string key,
            Dictionary<string, IEnumerator<T>> requestCache,
            Func<IEnumerator<T>> createRequest)
        {
            IEnumerator<T> iter;
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

        private IEnumerator<MetadataResponse> GetMetadata(MetadataRequest request, IProgress prog)
        {
            return GetCachedResponse(
                request.CacheID,
                metadataRequestCache,
                () => new TaskEnumerator<MetadataResponse>(request.Get(prog)));
        }

        public IEnumerator<MetadataResponse> GetMetadata(LatLngPoint position, IProgress prog = null)
        {
            var metadataRequest = new MetadataRequest(gmaps, position);
            return GetMetadata(metadataRequest, prog);
        }

        public IEnumerator<MetadataResponse> GetMetadata(PlaceName position, IProgress prog = null)
        {
            var metadataRequest = new MetadataRequest(gmaps, position);
            return GetMetadata(metadataRequest, prog);
        }

        private IEnumerator<TextureType> GetTextureAsync(PanoID pano, IProgress prog)
        {
            var imageRequest = new CrossCubeMapRequest(gmaps, pano, imageSize);
            var imageIter = new TaskEnumerator<ImageData>(imageRequest.GetJPEG(prog.Subdivide(0, 0.9f)));
            while (imageIter.MoveNext()) { yield return default; }
            var image = imageIter.Current;
            if (image != null)
            {
                var textureIter = createTexture(image, prog.Subdivide(0.9f, 0.1f));
                while (textureIter.MoveNext()) { yield return default; }
                var texture = textureIter.Current;
                yield return texture;
            }
        }

        public IEnumerator<TextureType> GetTexture(PanoID pano, IProgress prog = null)
        {
            return GetCachedResponse(
                (string)pano,
                textureRequestCache,
                () => GetTextureAsync(pano, prog));
        }
    }
}