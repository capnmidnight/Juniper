using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Juniper.HTTP.REST;

namespace Juniper.Google.Maps
{
    public class GoogleMapsRequestConfiguration : AbstractRequestConfiguration
    {
        private static readonly Uri gmaps = new Uri("https://maps.googleapis.com/maps/api/");

        private readonly string apiKey;
        private readonly string signingKey;

        public GoogleMapsRequestConfiguration(string apiKey, string signingKey, DirectoryInfo cacheLocation)
            : base(gmaps, cacheLocation)
        {
            this.apiKey = apiKey;
            this.signingKey = signingKey;
        }

        public GoogleMapsRequestConfiguration(string apiKey, string signingKey)
            : this(apiKey, signingKey, null) { }

        internal Uri AddKey(Uri uri)
        {
            var unsignedUriBuilder = new UriBuilder(uri);
            unsignedUriBuilder.AddQuery("key", apiKey);
            return unsignedUriBuilder.Uri;
        }

        internal Uri AddSignature(Uri unsignedUri)
        {
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
    }
}