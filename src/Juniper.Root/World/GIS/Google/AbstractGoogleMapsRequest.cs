using System;
using System.Security.Cryptography;
using System.Text;

using Juniper.HTTP;
using Juniper.HTTP.REST;

namespace Juniper.World.GIS.Google
{
    public interface IGoogleMapsRequest
    { }

    public abstract class AbstractGoogleMapsRequest<MediaTypeT> : AbstractRequest<MediaTypeT>, IGoogleMapsRequest
        where MediaTypeT : MediaType
    {
        private static readonly Uri gmaps = new Uri("https://maps.googleapis.com/maps/api/");

        private readonly string apiKey;
        private readonly string signingKey;

        protected AbstractGoogleMapsRequest(string path, string apiKey, string signingKey, MediaTypeT contentType)
            : base(HttpMethod.GET, AddPath(gmaps, path), contentType)
        {
            this.apiKey = apiKey;
            this.signingKey = signingKey;
        }

        protected override Uri AuthenticatedURI
        {
            get
            {
                var unsignedUri = base.AuthenticatedURI;
                var unsignedUriBuilder = new UriBuilder(unsignedUri);
                unsignedUriBuilder.AddQuery("key", apiKey);
                var unsignedUriWithKey = unsignedUriBuilder.Uri;
                if(string.IsNullOrEmpty(signingKey))
                {
                    return unsignedUriWithKey;
                }
                else
                {
                    var pkBytes = Convert.FromBase64String(signingKey.FromGoogleModifiedBase64());
                    using (var hasher = new HMACSHA1(pkBytes))
                    {
                        var urlBytes = Encoding.ASCII.GetBytes(unsignedUriWithKey.LocalPath + unsignedUriWithKey.Query);
                        var hash = hasher.ComputeHash(urlBytes);
                        var signature = Convert.ToBase64String(hash).ToGoogleModifiedBase64();

                        var signedUri = new UriBuilder(unsignedUriWithKey);
                        signedUri.AddQuery("signature", signature);
                        return signedUri.Uri;
                    }
                }
            }
        }
    }
}