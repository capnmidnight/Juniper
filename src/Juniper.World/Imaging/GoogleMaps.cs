using Juniper.Image;
using Juniper.Serialization;
using Juniper.World.GIS;
using System;
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

        public GoogleMaps(IDeserializer deserializer, string apiKey, string signingKey)
        {
            this.deserializer = deserializer;
            this.apiKey = apiKey;
            this.signingKey = signingKey;
        }

        private Uri Sign(Uri inputUri)
        {
            var pkBytes = Convert.FromBase64String(signingKey.FromGoogleModifiedBase64());
            var hasher = new HMACSHA1(pkBytes);

            var urlBytes = Encoding.ASCII.GetBytes(inputUri.LocalPath + inputUri.Query);
            var hash = hasher.ComputeHash(urlBytes);
            var signature = Convert.ToBase64String(hash).ToGoogleModifiedBase64();

            var outputUri = new UriBuilder(inputUri);
            outputUri.AddQuery("signature", signature);
            return outputUri.Uri;
        }

        private UriBuilder MakeUriBuilder(Uri uriBase, string locParam)
        {
            var builder = new UriBuilder(uriBase);
            builder.AddQuery(locParam);
            builder.AddQuery("key", apiKey);
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

        public async Task<Metadata> GetMetadata(PanoID pano)
        {
            return await GetMetadata($"pano={pano}");
        }

        public async Task<Metadata> GetMetadataAtLocation(LatLngPoint location)
        {
            return await GetMetadataAtLocation($"{location.Latitude},{location.Longitude}");
        }

        public async Task<Metadata> GetMetadataAtLocation(string location)
        {
            return await GetMetadata($"location={location}");
        }

        private async Task<Metadata> GetMetadata(string locParam)
        {
            var uri = Sign(MakeMetadatUri(locParam));
            var request = HttpWebRequestExt.Create(uri);
            var response = await request.Get();
            var body = response.ReadBodyString();
            return deserializer.Deserialize<Metadata>(body);
        }

        public async Task<RawImage> GetImage(PanoID pano, int width, int height, Heading heading, Pitch pitch, int searchRadius, bool outdoorOnly)
        {
            return await GetImage($"pano={pano}", width, height, heading, pitch, searchRadius, outdoorOnly);
        }

        public async Task<RawImage> GetImage(PanoID pano, int width, int height, Heading heading, Pitch pitch, int searchRadius)
        {
            return await GetImage($"pano={pano}", width, height, heading, pitch, searchRadius);
        }

        public async Task<RawImage> GetImage(PanoID pano, int width, int height, Heading heading, Pitch pitch, bool outdoorOnly)
        {
            return await GetImage($"pano={pano}", width, height, heading, pitch, outdoorOnly);
        }

        public async Task<RawImage> GetImage(PanoID pano, int width, int height, Heading heading, Pitch pitch)
        {
            return await GetImage($"pano={pano}", width, height, heading, pitch);
        }

        public async Task<RawImage> GetImage(PanoID pano, int width, int height, Heading heading, int searchRadius, bool outdoorOnly)
        {
            return await GetImage($"pano={pano}", width, height, heading, searchRadius, outdoorOnly);
        }

        public async Task<RawImage> GetImage(PanoID pano, int width, int height, Heading heading, int searchRadius)
        {
            return await GetImage($"pano={pano}", width, height, heading, searchRadius);
        }

        public async Task<RawImage> GetImage(PanoID pano, int width, int height, Heading heading, bool outdoorOnly)
        {
            return await GetImage($"pano={pano}", width, height, heading, outdoorOnly);
        }

        public async Task<RawImage> GetImage(PanoID pano, int width, int height, Heading heading)
        {
            return await GetImage($"pano={pano}", width, height, heading);
        }

        public async Task<RawImage> GetImage(PanoID pano, int width, int height, Pitch pitch, int searchRadius, bool outdoorOnly)
        {
            return await GetImage($"pano={pano}", width, height, pitch, searchRadius, outdoorOnly);
        }

        public async Task<RawImage> GetImage(PanoID pano, int width, int height, Pitch pitch, int searchRadius)
        {
            return await GetImage($"pano={pano}", width, height, pitch, searchRadius);
        }

        public async Task<RawImage> GetImage(PanoID pano, int width, int height, Pitch pitch, bool outdoorOnly)
        {
            return await GetImage($"pano={pano}", width, height, pitch, outdoorOnly);
        }

        public async Task<RawImage> GetImage(PanoID pano, int width, int height, Pitch pitch)
        {
            return await GetImage($"pano={pano}", width, height, pitch);
        }

        public async Task<RawImage> GetImage(PanoID pano, int width, int height, int searchRadius, bool outdoorOnly)
        {
            return await GetImage($"pano={pano}", width, height, searchRadius, outdoorOnly);
        }

        public async Task<RawImage> GetImage(PanoID pano, int width, int height, int searchRadius)
        {
            return await GetImage($"pano={pano}", width, height, searchRadius);
        }

        public async Task<RawImage> GetImage(PanoID pano, int width, int height, bool outdoorOnly)
        {
            return await GetImage($"pano={pano}", width, height, outdoorOnly);
        }

        public async Task<RawImage> GetImage(PanoID pano, int width, int height)
        {
            return await GetImage($"pano={pano}", width, height);
        }

        public async Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Heading heading, Pitch pitch, int searchRadius, bool outdoorOnly)
        {
            return await GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, heading, pitch, searchRadius, outdoorOnly);
        }

        public async Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Heading heading, Pitch pitch, int searchRadius)
        {
            return await GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, heading, pitch, searchRadius);
        }

        public async Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Heading heading, Pitch pitch, bool outdoorOnly)
        {
            return await GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, heading, pitch, outdoorOnly);
        }

        public async Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Heading heading, Pitch pitch)
        {
            return await GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, heading, pitch);
        }

        public async Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Heading heading, int searchRadius, bool outdoorOnly)
        {
            return await GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, heading, searchRadius, outdoorOnly);
        }

        public async Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Heading heading, int searchRadius)
        {
            return await GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, heading, searchRadius);
        }

        public async Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Heading heading, bool outdoorOnly)
        {
            return await GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, heading, outdoorOnly);
        }

        public async Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Heading heading)
        {
            return await GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, heading);
        }

        public async Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Pitch pitch, int searchRadius, bool outdoorOnly)
        {
            return await GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, pitch, searchRadius, outdoorOnly);
        }

        public async Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Pitch pitch, int searchRadius)
        {
            return await GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, pitch, searchRadius);
        }

        public async Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Pitch pitch, bool outdoorOnly)
        {
            return await GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, pitch, outdoorOnly);
        }

        public async Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, Pitch pitch)
        {
            return await GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, pitch);
        }

        public async Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, int searchRadius, bool outdoorOnly)
        {
            return await GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, searchRadius, outdoorOnly);
        }

        public async Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, int searchRadius)
        {
            return await GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, searchRadius);
        }

        public async Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height, bool outdoorOnly)
        {
            return await GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height, outdoorOnly);
        }

        public async Task<RawImage> GetImageAtLocation(LatLngPoint location, int width, int height)
        {
            return await GetImageAtLocation($"{location.Latitude},{location.Longitude}", width, height);
        }

        public async Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Heading heading, Pitch pitch, int searchRadius, bool outdoorOnly)
        {
            return await GetImage($"location={locationName}", width, height, heading, pitch, searchRadius, outdoorOnly);
        }

        public async Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Heading heading, Pitch pitch, int searchRadius)
        {
            return await GetImage($"location={locationName}", width, height, heading, pitch, searchRadius);
        }

        public async Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Heading heading, Pitch pitch, bool outdoorOnly)
        {
            return await GetImage($"location={locationName}", width, height, heading, pitch, outdoorOnly);
        }

        public async Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Heading heading, Pitch pitch)
        {
            return await GetImage($"location={locationName}", width, height, heading, pitch);
        }

        public async Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Heading heading, int searchRadius, bool outdoorOnly)
        {
            return await GetImage($"location={locationName}", width, height, heading, searchRadius, outdoorOnly);
        }

        public async Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Heading heading, int searchRadius)
        {
            return await GetImage($"location={locationName}", width, height, heading, searchRadius);
        }

        public async Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Heading heading, bool outdoorOnly)
        {
            return await GetImage($"location={locationName}", width, height, heading, outdoorOnly);
        }

        public async Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Heading heading)
        {
            return await GetImage($"location={locationName}", width, height, heading);
        }

        public async Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Pitch pitch, int searchRadius, bool outdoorOnly)
        {
            return await GetImage($"location={locationName}", width, height, pitch, searchRadius, outdoorOnly);
        }

        public async Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Pitch pitch, int searchRadius)
        {
            return await GetImage($"location={locationName}", width, height, pitch, searchRadius);
        }

        public async Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Pitch pitch, bool outdoorOnly)
        {
            return await GetImage($"location={locationName}", width, height, pitch, outdoorOnly);
        }

        public async Task<RawImage> GetImageAtLocation(string locationName, int width, int height, Pitch pitch)
        {
            return await GetImage($"location={locationName}", width, height, pitch);
        }

        public async Task<RawImage> GetImageAtLocation(string locationName, int width, int height, int searchRadius, bool outdoorOnly)
        {
            return await GetImage($"location={locationName}", width, height, searchRadius, outdoorOnly);
        }

        public async Task<RawImage> GetImageAtLocation(string locationName, int width, int height, int searchRadius)
        {
            return await GetImage($"location={locationName}", width, height, searchRadius);
        }

        public async Task<RawImage> GetImageAtLocation(string locationName, int width, int height, bool outdoorOnly)
        {
            return await GetImage($"location={locationName}", width, height, outdoorOnly);
        }

        public async Task<RawImage> GetImageAtLocation(string locationName, int width, int height)
        {
            return await GetImage($"location={locationName}", width, height);
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

        private async Task<RawImage> GetImage(string locParam, int width, int height, Heading heading, Pitch pitch, int searchRadius, bool outdoorOnly)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddHeading(builder, heading);
            AddPitch(builder, pitch);
            AddRadius(builder, searchRadius);
            AddSource(builder, outdoorOnly);

            return await GetImage(builder.Uri, signingKey);
        }

        private async Task<RawImage> GetImage(string locParam, int width, int height, Heading heading, Pitch pitch, int searchRadius)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddHeading(builder, heading);
            AddPitch(builder, pitch);
            AddRadius(builder, searchRadius);

            return await GetImage(builder.Uri, signingKey);
        }

        private async Task<RawImage> GetImage(string locParam, int width, int height, Heading heading, Pitch pitch, bool outdoorOnly)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddHeading(builder, heading);
            AddPitch(builder, pitch);
            AddSource(builder, outdoorOnly);

            return await GetImage(builder.Uri, signingKey);
        }

        private async Task<RawImage> GetImage(string locParam, int width, int height, Heading heading, Pitch pitch)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddHeading(builder, heading);
            AddPitch(builder, pitch);

            return await GetImage(builder.Uri, signingKey);
        }

        private async Task<RawImage> GetImage(string locParam, int width, int height, Heading heading, int searchRadius, bool outdoorOnly)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddHeading(builder, heading);
            AddRadius(builder, searchRadius);
            AddSource(builder, outdoorOnly);

            return await GetImage(builder.Uri, signingKey);
        }

        private async Task<RawImage> GetImage(string locParam, int width, int height, Heading heading, int searchRadius)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddHeading(builder, heading);
            AddRadius(builder, searchRadius);

            return await GetImage(builder.Uri, signingKey);
        }

        private async Task<RawImage> GetImage(string locParam, int width, int height, Heading heading, bool outdoorOnly)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddHeading(builder, heading);
            AddSource(builder, outdoorOnly);

            return await GetImage(builder.Uri, signingKey);
        }

        private async Task<RawImage> GetImage(string locParam, int width, int height, Heading heading)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddHeading(builder, heading);

            return await GetImage(builder.Uri, signingKey);
        }

        private async Task<RawImage> GetImage(string locParam, int width, int height, Pitch pitch, int searchRadius, bool outdoorOnly)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddPitch(builder, pitch);
            AddRadius(builder, searchRadius);
            AddSource(builder, outdoorOnly);

            return await GetImage(builder.Uri, signingKey);
        }

        private async Task<RawImage> GetImage(string locParam, int width, int height, Pitch pitch, int searchRadius)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddPitch(builder, pitch);
            AddRadius(builder, searchRadius);

            return await GetImage(builder.Uri, signingKey);
        }

        private async Task<RawImage> GetImage(string locParam, int width, int height, Pitch pitch, bool outdoorOnly)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddPitch(builder, pitch);
            AddSource(builder, outdoorOnly);

            return await GetImage(builder.Uri, signingKey);
        }

        private async Task<RawImage> GetImage(string locParam, int width, int height, Pitch pitch)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddPitch(builder, pitch);

            return await GetImage(builder.Uri, signingKey);
        }

        private async Task<RawImage> GetImage(string locParam, int width, int height, int searchRadius, bool outdoorOnly)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddRadius(builder, searchRadius);
            AddSource(builder, outdoorOnly);

            return await GetImage(builder.Uri, signingKey);
        }

        private async Task<RawImage> GetImage(string locParam, int width, int height, int searchRadius)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddRadius(builder, searchRadius);

            return await GetImage(builder.Uri, signingKey);
        }

        private async Task<RawImage> GetImage(string locParam, int width, int height, bool outdoorOnly)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            AddSource(builder, outdoorOnly);

            return await GetImage(builder.Uri, signingKey);
        }

        private async Task<RawImage> GetImage(string locParam, int width, int height)
        {
            var builder = MakeImageUriBuilder(locParam, width, height);
            return await GetImage(builder.Uri, signingKey);
        }

        private async Task<RawImage> GetImage(Uri uri, string signingKey)
        {
            var request = HttpWebRequestExt.Create(Sign(uri));
            var response = await request.Get();
            return await Image.Decoder.DecodeResponseAsync(response);
        }
    }
}