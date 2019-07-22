using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Juniper.HTTP.REST;
using Juniper.Serialization;

namespace Juniper.Google.Maps
{
    public class Endpoint : AbstractEndpoint
    {
        private readonly string apiKey;
        private readonly string signingKey;

        public Endpoint(IDeserializer deserializer, string apiKey, string signingKey, DirectoryInfo cacheLocation = null)
            : base(deserializer, cacheLocation)
        {
            this.apiKey = apiKey;
            this.signingKey = signingKey;
        }



        internal Uri AddCredentials(Uri uri, bool sign)
        {
            var unsignedUriBuilder = new UriBuilder(uri);
            unsignedUriBuilder.AddQuery("key", apiKey);
            var unsignedUri = unsignedUriBuilder.Uri;
            if (!sign)
            {
                return unsignedUri;
            }
            else
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
}