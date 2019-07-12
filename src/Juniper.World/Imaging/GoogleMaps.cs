using Juniper.Image;
using Juniper.Serialization;
using Juniper.World.GIS;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.World.Imaging
{

    public class GoogleMaps
    {
        public enum Heading
        {
            North = 0,
            East = 90,
            South = 180,
            West = 270
        }

        public enum Pitch
        {
            Down = -90,
            Level = 0,
            Up = 90
        }

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

        public struct PanoID
        {
            private readonly string id;

            public PanoID(string id)
            {
                this.id = id;
            }

            public override string ToString()
            {
                return id;
            }

            public override int GetHashCode()
            {
                return id.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is PanoID pano && pano.id == id;
            }
        }

        [Serializable]
        public class Metadata : ISerializable
        {
            public readonly StatusCode status;
            public readonly string error_message;
            public readonly string copyright;
            public readonly string date;
            public readonly PanoID pano_id;
            public readonly LatLngPoint location;

            public Metadata(SerializationInfo info, StreamingContext context)
            {
                status = info.GetEnumFromString<StatusCode>(nameof(status));
                if (status == StatusCode.OK)
                {
                    copyright = info.GetString(nameof(copyright));
                    date = info.GetString(nameof(date));
                    pano_id = new PanoID(info.GetString(nameof(pano_id)));
                    location = info.GetValue<LatLngPoint>(nameof(location));
                }
                else
                {
                    error_message = info.GetString(nameof(error_message));
                }
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(status), status.ToString());
                info.MaybeAddValue(nameof(error_message), error_message);
                info.MaybeAddValue(nameof(copyright), copyright);
                info.MaybeAddValue(nameof(date), date);
                info.MaybeAddValue(nameof(pano_id), pano_id.ToString());
                info.MaybeAddValue(nameof(location), new
                {
                    lat = location.Latitude,
                    lng = location.Longitude
                });
            }
        }


        public abstract class Search
        {
            protected const string baseServiceURI = "https://maps.googleapis.com/maps/api/streetview";

            protected static string MakeCacheID(Uri uri, string extension)
            {
                var cacheID = string.Join("_", uri.PathAndQuery
                    .Substring(1)
                    .Split(Path.GetInvalidFileNameChars()));
                cacheID = Path.ChangeExtension(cacheID, extension);
                return cacheID;
            }

            protected readonly UriBuilder uriBuilder;
            private string extension;

            private Search(Uri baseUri, string extension)
            {
                uriBuilder = new UriBuilder(baseUri);
                this.extension = extension;
            }

            public Search(Uri baseUri, string extension, PanoID pano)
                : this(baseUri, extension)
            {
                uriBuilder.AddQuery("pano", pano);
            }

            public Search(Uri baseUri, string extension, string placeName)
                : this(baseUri, extension)
            {
                uriBuilder.AddQuery("location", placeName);
            }

            public Search(Uri baseUri, string extension, LatLngPoint location)
                : this(baseUri, extension)
            {
                uriBuilder.AddQuery("location", $"{location.Latitude},{location.Longitude}");
            }

            public Uri Uri
            {
                get
                {
                    return uriBuilder.Uri;
                }
            }

            public string CacheFileName
            {
                get
                {
                    return MakeCacheID(Uri, extension);
                }
            }
        }

        public class MetadataSearch : Search
        {
            private static readonly Uri metadataURIBase = new Uri(baseServiceURI + "/metadata");

            public MetadataSearch(PanoID pano)
                : base(metadataURIBase, "json", pano) { }

            public MetadataSearch(string placeName)
                : base(metadataURIBase, "json", placeName) { }

            public MetadataSearch(LatLngPoint location)
                : base(metadataURIBase, "json", location) { }
        }

        public class ImageSearch : Search
        {
            private static readonly Uri imageURIBase = new Uri(baseServiceURI);

            public ImageSearch(PanoID pano, int width, int height)
                : base(imageURIBase, "jpeg", pano)
            {
                SetSize(width, height);
            }

            public ImageSearch(string placeName, int width, int height)
                : base(imageURIBase, "jpeg", placeName)
            {
                SetSize(width, height);
            }

            public ImageSearch(LatLngPoint location, int width, int height)
                : base(imageURIBase, "jpeg", location)
            {
                SetSize(width, height);
            }

            private void SetSize(int width, int height)
            {
                uriBuilder.AddQuery("size", $"{width}x{height}");
            }

            public ImageSearch AddHeading(Heading heading)
            {
                uriBuilder.AddQuery("heading", (int)heading);
                return this;
            }

            public ImageSearch AddPitch(Pitch pitch)
            {
                uriBuilder.AddQuery("pitch", (int)pitch);
                return this;
            }

            public ImageSearch AddRadius(int searchRadius)
            {
                uriBuilder.AddQuery("radius", searchRadius);
                return this;
            }

            public ImageSearch AddSource(bool outdoorOnly)
            {
                if (outdoorOnly)
                {
                    uriBuilder.AddQuery("source=outdoor");
                }
                return this;
            }
        }

        public class CubeMapSearch
        {
            private CubeMapSearch(ImageSearch[] images)
            {
                Images = images;

                images[0].AddHeading(Heading.North).AddPitch(Pitch.Level);
                images[1].AddHeading(Heading.East).AddPitch(Pitch.Level);
                images[2].AddHeading(Heading.West).AddPitch(Pitch.Level);
                images[3].AddHeading(Heading.South).AddPitch(Pitch.Level);
                images[4].AddHeading(Heading.North).AddPitch(Pitch.Up);
                images[5].AddHeading(Heading.North).AddPitch(Pitch.Down);
            }

            public CubeMapSearch(PanoID pano, int width, int height)
                : this(new[]
                {
                    new ImageSearch(pano, width, height),
                    new ImageSearch(pano, width, height),
                    new ImageSearch(pano, width, height),
                    new ImageSearch(pano, width, height),
                    new ImageSearch(pano, width, height),
                    new ImageSearch(pano, width, height),
                })
            {
            }

            public CubeMapSearch(string placeName, int width, int height)
                : this(new[]
                {
                    new ImageSearch(placeName, width, height),
                    new ImageSearch(placeName, width, height),
                    new ImageSearch(placeName, width, height),
                    new ImageSearch(placeName, width, height),
                    new ImageSearch(placeName, width, height),
                    new ImageSearch(placeName, width, height),
                })
            {
            }

            public CubeMapSearch(LatLngPoint location, int width, int height)
                : this(new[]
                {
                    new ImageSearch(location, width, height),
                    new ImageSearch(location, width, height),
                    new ImageSearch(location, width, height),
                    new ImageSearch(location, width, height),
                    new ImageSearch(location, width, height),
                    new ImageSearch(location, width, height),
                })
            {
            }

            public CubeMapSearch AddRadius(int searchRadius)
            {
                foreach (var img in Images)
                {
                    img.AddRadius(searchRadius);
                }
                return this;
            }

            public CubeMapSearch AddSource(bool outdoorOnly)
            {
                foreach (var img in Images)
                {
                    img.AddSource(outdoorOnly);
                }
                return this;
            }

            public ImageSearch[] Images { get; }

            public Uri[] Uris
            {
                get
                {
                    return Images.Select(i => i.Uri).ToArray();
                }
            }

            public string[] CacheFileNames
            {
                get
                {
                    return Images.Select(i => i.CacheFileName).ToArray();
                }
            }
        }

        private readonly IDeserializer deserializer;
        private readonly string apiKey;
        private readonly string signingKey;
        private readonly DirectoryInfo cacheLocation;

        public GoogleMaps(IDeserializer deserializer, string apiKey, string signingKey, DirectoryInfo cacheLocation = null)
        {
            this.deserializer = deserializer;
            this.apiKey = apiKey;
            this.signingKey = signingKey;
            this.cacheLocation = cacheLocation;
            cacheLocation?.Create();
        }

        private Uri Sign(Uri uri)
        {
            var unsignedUriBuilder = new UriBuilder(uri);
            unsignedUriBuilder.AddQuery("key", apiKey);
            var unsignedUri = unsignedUriBuilder.Uri;

            var pkBytes = Convert.FromBase64String(signingKey.FromGoogleModifiedBase64());
            var hasher = new HMACSHA1(pkBytes);

            var urlBytes = Encoding.ASCII.GetBytes(unsignedUri.LocalPath + unsignedUri.Query);
            var hash = hasher.ComputeHash(urlBytes);
            var signature = Convert.ToBase64String(hash).ToGoogleModifiedBase64();

            var signedUri = new UriBuilder(unsignedUri);
            signedUri.AddQuery("signature", signature);
            return signedUri.Uri;
        }

        private string MakeFullCachePath(string cacheFileName)
        {
            if (cacheLocation == null)
            {
                return null;
            }
            else
            {
                return Path.Combine(cacheLocation.FullName, cacheFileName);
            }
        }

        public bool IsCached(MetadataSearch search)
        {
            return File.Exists(MakeFullCachePath(search.CacheFileName));
        }

        public bool IsCached(ImageSearch search)
        {
            return File.Exists(MakeFullCachePath(search.CacheFileName));
        }

        public bool IsCached(CubeMapSearch search)
        {
            return search.CacheFileNames
                .Select(MakeFullCachePath)
                .All(File.Exists);
        }

        private Task<T> Get<T>(Search search, Func<Stream, T> decode)
        {
            return HttpWebRequestExt.CachedGet(
                Sign(search.Uri),
                decode,
                MakeFullCachePath(search.CacheFileName));
        }

        public Task<Metadata> Get(MetadataSearch search)
        {
            return Get(search, deserializer.Deserialize<Metadata>);
        }

        public Task<RawImage> Get(ImageSearch search)
        {
            return Get(search, Image.Decoder.DecodeJPEG);
        }

        public async Task<RawImage[]> Get(CubeMapSearch search)
        {
            var tasks = search.Images.Select(Get);
            await Task.WhenAll(tasks);
            return tasks.Select(t => t.Result).ToArray();
        }
    }
}