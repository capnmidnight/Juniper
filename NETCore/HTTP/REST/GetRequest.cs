namespace Juniper.HTTP.REST;

public class GetRequest : AbstractRequest<MediaType>
{
    public GetRequest(HttpClient http, Uri url, MediaType contentType)
        : base(http, HttpMethod.Get, url, contentType)
    { }

    protected override string InternalCacheID =>
        StandardRequestCacheID;
}
