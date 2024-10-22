using Juniper.HTTP.REST;
using System.Security.Cryptography;
using System.Text;

namespace Juniper.World.GIS.Google;

public abstract class AbstractGoogleMapsRequest<MediaTypeT> : AbstractRequest<MediaTypeT>, IGoogleMapsRequest
    where MediaTypeT : MediaType
{
    private static readonly Uri gmaps = new("https://maps.googleapis.com/maps/api/");

    private readonly string apiKey;
    private readonly string? signingKey;

    protected AbstractGoogleMapsRequest(HttpClient http, string path, MediaTypeT contentType, string apiKey, string? signingKey)
        : base(http, HttpMethod.Get, AddPath(gmaps, path), contentType)
    {
        this.apiKey = apiKey;
        this.signingKey = signingKey;
    }

    protected override string InternalCacheID =>
        BaseURI.Query[1..];

    protected override Uri AuthenticatedURI
    {
        get
        {
            var unsignedUri = base.AuthenticatedURI;
            var unsignedUriBuilder = new UriBuilder(unsignedUri);
            unsignedUriBuilder.AddQuery("key", apiKey);
            var unsignedUriWithKey = unsignedUriBuilder.Uri;
            if (string.IsNullOrEmpty(signingKey))
            {
                return unsignedUriWithKey;
            }
            else
            {
                var pkBytes = Convert.FromBase64String(signingKey.FromGoogleModifiedBase64());
                // Google requires use of this particular hashing algorithm.
                using var hasher = new HMACSHA1(pkBytes);
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