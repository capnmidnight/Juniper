using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Juniper.Serialization;

namespace Juniper.Google
{
    public class AbstractAPI
    {
        private readonly IDeserializer deserializer;
        private readonly string apiKey;
        private readonly string signingKey;
        internal readonly DirectoryInfo cacheLocation;

        protected AbstractAPI(IDeserializer deserializer, string apiKey, string signingKey, DirectoryInfo cacheLocation = null)
        {
            this.deserializer = deserializer;
            this.apiKey = apiKey;
            this.signingKey = signingKey;
            this.cacheLocation = cacheLocation;
            cacheLocation?.Create();
        }

        protected AbstractAPI(IDeserializer deserializer, string apiKey, string signingKey, string cacheDirectoryName = null)
            : this(deserializer, apiKey, signingKey, cacheDirectoryName == null ? null : new DirectoryInfo(cacheDirectoryName))
        {
        }

        internal Uri Sign(Uri uri)
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

        public bool IsCached<T, U>(AbstractSearch<T, U> search)
        {
            return search.IsCached(this);
        }

        public Task<U> Get<T, U>(AbstractSearch<T, U> search)
        {
            return search.Get(this);
        }

        internal T DecodeObject<T>(Stream stream)
        {
            return deserializer.Deserialize<T>(stream);
        }
    }
}
