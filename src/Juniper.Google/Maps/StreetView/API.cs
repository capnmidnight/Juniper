using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Juniper.Image;
using Juniper.Serialization;

namespace Juniper.Google.Maps.StreetView
{
    public class API
    {
        public enum StatusCode
        {
            OK,
            ZERO_RESULTS,
            NOT_FOUND,
            OVER_QUERY_LIMIT,
            REQUEST_DENIED,
            INVALID_REQUEST,
            UNKOWN_ERROR
        }

        private readonly IDeserializer deserializer;
        private readonly string apiKey;
        private readonly string signingKey;
        private readonly DirectoryInfo cacheLocation;

        public API(IDeserializer deserializer, string apiKey, string signingKey, DirectoryInfo cacheLocation = null)
        {
            this.deserializer = deserializer;
            this.apiKey = apiKey;
            this.signingKey = signingKey;
            this.cacheLocation = cacheLocation;
            cacheLocation?.Create();
        }

        public API(IDeserializer deserializer, string apiKey, string signingKey, string cacheDirectoryName = null)
            : this(deserializer, apiKey, signingKey, cacheDirectoryName == null ? null : new DirectoryInfo(cacheDirectoryName))
        {
        }

        private Uri Sign(Uri uri)
        {
            var unsignedUriBuilder = new UriBuilder(uri);
            unsignedUriBuilder.AddQuery("key", apiKey);
            var unsignedUri = unsignedUriBuilder.Uri;

            var pkBytes = Convert.FromBase64String(signingKey.FromGoogleModifiedBase64());
            using (var hasher = new HMACSHA1(pkBytes))
            {

                var urlBytes = Encoding.ASCII.GetBytes(unsignedUri.LocalPath + unsignedUri.Query);
                var hash = hasher.ComputeHash(urlBytes);
                var signature = Convert.ToBase64String(hash).ToGoogleModifiedBase64();

                var signedUri = new UriBuilder(unsignedUri);
                signedUri.AddQuery("signature", signature);
                return signedUri.Uri;
            }
        }

        private FileInfo MakeFullCachePath(AbstractStreetViewSearch search)
        {
            if (search == null)
            {
                return null;
            }
            else
            {
                var cacheID = string.Join("_", search.Uri.PathAndQuery
                    .Substring(1)
                    .Split(Path.GetInvalidFileNameChars()));
                var path = Path.Combine(cacheLocation.FullName, search.locString, cacheID);
                if (!search.extension.StartsWith("."))
                {
                    path += ".";
                }
                path += search.extension;
                return new FileInfo(path);
            }
        }

        public bool IsCached(MetadataSearch search)
        {
            return MakeFullCachePath(search).Exists;
        }

        public bool IsCached(ImageSearch search)
        {
            return MakeFullCachePath(search).Exists;
        }

        public bool IsCached(CubeMapSearch search)
        {
            return search.SubSearches
                .Select(MakeFullCachePath)
                .All(f => f.Exists);
        }

        private Task<T> Get<T>(AbstractStreetViewSearch search, Func<Stream, T> decode)
        {
            return Task.Run(() => HttpWebRequestExt.CachedGet(
                Sign(search.Uri),
                decode,
                MakeFullCachePath(search)));
        }

        public Task<Metadata> Get(MetadataSearch search)
        {
            return Get(search, deserializer.Deserialize<Metadata>);
        }

        private async Task<RawImage> GetAsync(ImageSearch search, bool flip)
        {
            var image = await Get(search, Image.Decoder.DecodeJPEG);
            if (flip)
            {
                await image.FlipAsync();
            }
            return image;
        }

        public Task<RawImage> Get(ImageSearch search, bool flip = false)
        {
            return Task.Run(() => GetAsync(search, flip));
        }

        public Task<RawImage[]> Get(CubeMapSearch search, bool flip = false)
        {
            return Task.WhenAll(search
                .SubSearches
                .Select(s => GetAsync(s, flip)));
        }
    }
}