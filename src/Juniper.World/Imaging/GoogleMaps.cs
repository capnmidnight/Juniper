using Juniper.Image;
using Juniper.Serialization;
using Juniper.World.GIS;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.Imaging
{

    public class GoogleMaps
    {
        public enum Heading
        {
            North = 0,
            East = 90,
            South = 180,
            West = 360
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
            public readonly string copyright;
            public readonly string date;
            public readonly PanoID pano_id;
            public readonly MetadataLocation location;

            public Metadata(SerializationInfo info, StreamingContext context)
            {
                status = info.GetEnumFromString<StatusCode>(nameof(status));
                if(status == StatusCode.OK)
                {
                    copyright = info.GetString(nameof(copyright));
                    date = info.GetString(nameof(date));
                    pano_id = new PanoID(info.GetString(nameof(pano_id)));
                    location = info.GetValue<MetadataLocation>(nameof(location));
                }
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(status), status.ToString());
                info.MaybeAddValue(nameof(copyright), copyright);
                info.MaybeAddValue(nameof(date), date);
                info.MaybeAddValue(nameof(pano_id), pano_id.ToString());
                info.MaybeAddValue(nameof(location), location);
            }
        }

        [Serializable]
        public class MetadataLocation : ISerializable
        {
            public readonly double lat;
            public readonly double lng;

            public MetadataLocation(SerializationInfo info, StreamingContext context)
            {
                lat = info.GetDouble(nameof(lat));
                lng = info.GetDouble(nameof(lng));
            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(lat), lat);
                info.AddValue(nameof(lng), lng);
            }
        }

        private const string baseURI = "https://maps.googleapis.com/maps/api/streetview";
        private static readonly Uri imageURIBase = new Uri(baseURI);
        private static readonly Uri metadataURIBase = new Uri(baseURI + "/metadata");

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

        private UriBuilder MakeUriBuilder(Uri uriBase, string locParam)
        {
            var builder = new UriBuilder(uriBase);
            builder.AddQuery(locParam);
            return builder;
        }

        private Uri MakeMetadatUri(string locParam)
        {
            return MakeUriBuilder(metadataURIBase, locParam).Uri;
        }

        private UriBuilder MakeImageUriBuilder(string locParam, int width, int height)
        {
            var builder = MakeUriBuilder(imageURIBase, locParam);
            builder.AddQuery("size", $"{width}x{height}");
            return builder;
        }

        private static string MakeCacheID(Uri uri, string extension)
        {
            var cacheID = uri.PathAndQuery.Replace('/', '_');
            cacheID = Path.ChangeExtension(cacheID, extension);
            return cacheID;
        }

        private Task<Metadata> GetMetadata(string locParam)
        {
            var uri = MakeMetadatUri(locParam);
            return HttpWebRequestExt.CachedGet(
                Sign(uri),
                cacheLocation,
                MakeCacheID(uri, "json"),
                deserializer.Deserialize<Metadata>);
        }

        private Task<RawImage> GetImage(Uri uri)
        {
            return HttpWebRequestExt.CachedGet(
                Sign(uri),
                cacheLocation,
                MakeCacheID(uri, "jpeg"),
                Image.Decoder.DecodeJPEG);
        }

        public Task<Metadata> GetMetadata(PanoID pano)
        {
            return GetMetadata($"pano={pano}");
        }

        public Task<Metadata> GetMetadataAtLocation(LatLngPoint location)
        {
            return GetMetadataAtLocation($"{location.Latitude},{location.Longitude}");
        }

        public Task<Metadata> GetMetadataAtLocation(string location)
        {
            return GetMetadata($"location={location}");
        }

        public Task<RawImage> GetImage(PanoID pano, int width, int height, Heading heading, Pitch pitch, int searchRadius, bool outdoorOnly)
        {
            return GetImage($"pano={pano}", width, height, heading, pitch, searchRadius, outdoorOnly);
        }

        public Task<RawImage> GetImage(PanoID pano, int width, int height, Heading heading, Pitch pitch, int searchRadius)
        {
            return GetImage($"pano={pano}", width, height, heading, pitch, searchRadius);
        }

        public Task<RawImage> GetImage(PanoID pano, int width, int height, Heading heading, Pitch pitch, bool outdoorOnly)
        {
            return GetImage($"pano={pano}", width, height, heading, pitch, outdoorOnly);
        }

        public Task<RawImage> GetImage(PanoID pano, int width, int height, Heading heading, Pitch pitch)
        {
            return GetImage($"pano={pano}", width, height, heading, pitch);
        }

        public Task<RawImage> GetImage(PanoID pano, int width, int height, Heading heading, int searchRadius, bool outdoorOnly)
        {
            return GetImage($"pano={pano}", width, height, heading, searchRadius, outdoorOnly);
        }

        public Task<RawImage> GetImage(PanoID pano, int width, int height, Heading heading, int searchRadius)
        {
            return GetImage($"pano={pano}", width, height, heading, searchRadius);
        }

        public Task<RawImage> GetImage(PanoID pano, int width, int height, Heading heading, bool outdoorOnly)
        {
            return GetImage($"pano={pano}", width, height, heading, outdoorOnly);
        }

        public Task<RawImage> GetImage(PanoID pano, int width, int height, Heading heading)
        {
            return GetImage($"pano={pano}", width, height, heading);
        }

        public Task<RawImage> GetImage(PanoID pano, int width, int height, Pitch pitch, int searchRadius, bool outdoorOnly)
        {
            return GetImage($"pano={pano}", width, height, pitch, searchRadius, outdoorOnly);
        }

        public Task<RawImage> GetImage(PanoID pano, int width, int height, Pitch pitch, int searchRadius)
        {
            return GetImage($"pano={pano}", width, height, pitch, searchRadius);
        }

        public Task<RawImage> GetImage(PanoID pano, int width, int height, Pitch pitch, bool outdoorOnly)
        {
            return GetImage($"pano={pano}", width, height, pitch, outdoorOnly);
        }

        public Task<RawImage> GetImage(PanoID pano, int width, int height, Pitch pitch)
        {
            return GetImage($"pano={pano}", width, height, pitch);
        }

        public Task<RawImage> GetImage(PanoID pano, int width, int height, int searchRadius, bool outdoorOnly)
        {
            return GetImage($"pano={pano}", width, height, searchRadius, outdoorOnly);
        }

        public Task<RawImage> GetImage(PanoID pano, int width, int height, int searchRadius)
        {
            return GetImage($"pano={pano}", width, height, searchRadius);
        }

        public Task<RawImage> GetImage(PanoID pano, int width, int height, bool outdoorOnly)
        {
            return GetImage($"pano={pano}", width, height, outdoorOnly);
        }

        public Task<RawImage> GetImage(PanoID pano, int width, int height)
        {
            return GetImage($"pano={pano}", width, height);
        }

        public Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Heading heading, Pitch pitch, int searchRadius, bool outdoorOnly)
        {
            return GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, heading, pitch, searchRadius, outdoorOnly);
        }

        public Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Heading heading, Pitch pitch, int searchRadius)
        {
            return GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, heading, pitch, searchRadius);
        }

        public Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Heading heading, Pitch pitch, bool outdoorOnly)
        {
            return GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, heading, pitch, outdoorOnly);
        }

        public Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Heading heading, Pitch pitch)
        {
            return GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, heading, pitch);
        }

        public Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Heading heading, int searchRadius, bool outdoorOnly)
        {
            return GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, heading, searchRadius, outdoorOnly);
        }

        public Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Heading heading, int searchRadius)
        {
            return GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, heading, searchRadius);
        }

        public Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Heading heading, bool outdoorOnly)
        {
            return GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, heading, outdoorOnly);
        }

        public Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Heading heading)
        {
            return GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, heading);
        }

        public Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Pitch pitch, int searchRadius, bool outdoorOnly)
        {
            return GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, pitch, searchRadius, outdoorOnly);
        }

        public Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Pitch pitch, int searchRadius)
        {
            return GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, pitch, searchRadius);
        }

        public Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Pitch pitch, bool outdoorOnly)
        {
            return GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, pitch, outdoorOnly);
        }

        public Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Pitch pitch)
        {
            return GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, pitch);
        }

        public Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, int searchRadius, bool outdoorOnly)
        {
            return GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, searchRadius, outdoorOnly);
        }

        public Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, int searchRadius)
        {
            return GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, searchRadius);
        }

        public Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, bool outdoorOnly)
        {
            return GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, outdoorOnly);
        }

        public Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height)
        {
            return GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height);
        }

        public Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Heading heading, Pitch pitch, int searchRadius, bool outdoorOnly)
        {
            return GetImage($"location={locationName}", width, height, heading, pitch, searchRadius, outdoorOnly);
        }

        public Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Heading heading, Pitch pitch, int searchRadius)
        {
            return GetImage($"location={locationName}", width, height, heading, pitch, searchRadius);
        }

        public Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Heading heading, Pitch pitch, bool outdoorOnly)
        {
            return GetImage($"location={locationName}", width, height, heading, pitch, outdoorOnly);
        }

        public Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Heading heading, Pitch pitch)
        {
            return GetImage($"location={locationName}", width, height, heading, pitch);
        }

        public Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Heading heading, int searchRadius, bool outdoorOnly)
        {
            return GetImage($"location={locationName}", width, height, heading, searchRadius, outdoorOnly);
        }

        public Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Heading heading, int searchRadius)
        {
            return GetImage($"location={locationName}", width, height, heading, searchRadius);
        }

        public Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Heading heading, bool outdoorOnly)
        {
            return GetImage($"location={locationName}", width, height, heading, outdoorOnly);
        }

        public Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Heading heading)
        {
            return GetImage($"location={locationName}", width, height, heading);
        }

        public Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Pitch pitch, int searchRadius, bool outdoorOnly)
        {
            return GetImage($"location={locationName}", width, height, pitch, searchRadius, outdoorOnly);
        }

        public Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Pitch pitch, int searchRadius)
        {
            return GetImage($"location={locationName}", width, height, pitch, searchRadius);
        }

        public Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Pitch pitch, bool outdoorOnly)
        {
            return GetImage($"location={locationName}", width, height, pitch, outdoorOnly);
        }

        public Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Pitch pitch)
        {
            return GetImage($"location={locationName}", width, height, pitch);
        }

        public Task<RawImage> GetImageAtLocation(string locationName, int width, int height, int searchRadius, bool outdoorOnly)
        {
            return GetImage($"location={locationName}", width, height, searchRadius, outdoorOnly);
        }

        public Task<RawImage> GetImageAtLocation(string locationName, int width, int height, int searchRadius)
        {
            return GetImage($"location={locationName}", width, height, searchRadius);
        }

        public Task<RawImage> GetImageAtLocation(string locationName, int width, int height, bool outdoorOnly)
        {
            return GetImage($"location={locationName}", width, height, outdoorOnly);
        }

        public Task<RawImage> GetImageAtLocation(string locationName, int width, int height)
        {
            return GetImage($"location={locationName}", width, height);
        }

        private static void AddHeading(UriBuilder builder, Heading heading)
        {
            builder.AddQuery("heading", (int)heading);
        }

        private static void AddPitch(UriBuilder builder, Pitch pitch)
        {
            builder.AddQuery("pitch", (int)pitch);
        }

        private static void AddRadius(UriBuilder builder, int searchRadius)
        {
            builder.AddQuery("radius", searchRadius);
        }

        private static void AddSource(UriBuilder builder, bool outdoorOnly)
        {
            if (outdoorOnly)
            {
                builder.AddQuery("source=outdoor");
            }
        }

        private Task<RawImage> GetImage(string locParam, int width, int height, Heading heading, Pitch pitch, int searchRadius, bool outdoorOnly)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddHeading(builder, heading);
            AddPitch(builder, pitch);
            AddRadius(builder, searchRadius);
            AddSource(builder, outdoorOnly);

            return GetImage(builder.Uri);
        }

        private Task<RawImage> GetImage(string locParam, int width, int height, Heading heading, Pitch pitch, int searchRadius)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddHeading(builder, heading);
            AddPitch(builder, pitch);
            AddRadius(builder, searchRadius);

            return GetImage(builder.Uri);
        }

        private Task<RawImage> GetImage(string locParam, int width, int height, Heading heading, Pitch pitch, bool outdoorOnly)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddHeading(builder, heading);
            AddPitch(builder, pitch);
            AddSource(builder, outdoorOnly);

            return GetImage(builder.Uri);
        }

        private Task<RawImage> GetImage(string locParam, int width, int height, Heading heading, Pitch pitch)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddHeading(builder, heading);
            AddPitch(builder, pitch);

            return GetImage(builder.Uri);
        }

        private Task<RawImage> GetImage(string locParam, int width, int height, Heading heading, int searchRadius, bool outdoorOnly)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddHeading(builder, heading);
            AddRadius(builder, searchRadius);
            AddSource(builder, outdoorOnly);

            return GetImage(builder.Uri);
        }

        private Task<RawImage> GetImage(string locParam, int width, int height, Heading heading, int searchRadius)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddHeading(builder, heading);
            AddRadius(builder, searchRadius);

            return GetImage(builder.Uri);
        }

        private Task<RawImage> GetImage(string locParam, int width, int height, Heading heading, bool outdoorOnly)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddHeading(builder, heading);
            AddSource(builder, outdoorOnly);

            return GetImage(builder.Uri);
        }

        private Task<RawImage> GetImage(string locParam, int width, int height, Heading heading)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddHeading(builder, heading);

            return GetImage(builder.Uri);
        }

        private Task<RawImage> GetImage(string locParam, int width, int height, Pitch pitch, int searchRadius, bool outdoorOnly)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddPitch(builder, pitch);
            AddRadius(builder, searchRadius);
            AddSource(builder, outdoorOnly);

            return GetImage(builder.Uri);
        }

        private Task<RawImage> GetImage(string locParam, int width, int height, Pitch pitch, int searchRadius)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddPitch(builder, pitch);
            AddRadius(builder, searchRadius);

            return GetImage(builder.Uri);
        }

        private Task<RawImage> GetImage(string locParam, int width, int height, Pitch pitch, bool outdoorOnly)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddPitch(builder, pitch);
            AddSource(builder, outdoorOnly);

            return GetImage(builder.Uri);
        }

        private Task<RawImage> GetImage(string locParam, int width, int height, Pitch pitch)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddPitch(builder, pitch);

            return GetImage(builder.Uri);
        }

        private Task<RawImage> GetImage(string locParam, int width, int height, int searchRadius, bool outdoorOnly)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddRadius(builder, searchRadius);
            AddSource(builder, outdoorOnly);

            return GetImage(builder.Uri);
        }

        private Task<RawImage> GetImage(string locParam, int width, int height, int searchRadius)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddRadius(builder, searchRadius);

            return GetImage(builder.Uri);
        }

        private Task<RawImage> GetImage(string locParam, int width, int height, bool outdoorOnly)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddSource(builder, outdoorOnly);

            return GetImage(builder.Uri);
        }

        private Task<RawImage> GetImage(string locParam, int width, int height)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            return GetImage(builder.Uri);
        }
    }
}