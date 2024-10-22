using Juniper.Progress;

using System.Net.Http;

namespace Juniper.Speech.Azure;

public class AuthTokenRequest : AbstractAzureRequest<MediaType.Text>
{
    private readonly string subscriptionKey;

    public AuthTokenRequest(HttpClient http, string region, string subscriptionKey)
        : base(http, HttpMethod.Post, region, "api.cognitive", "sts/v1.0/issueToken", MediaType.Text_Plain)
    {
        this.subscriptionKey = subscriptionKey;
    }

    protected override string InternalCacheID => null;

    protected override void ModifyRequest(HttpRequestMessage request, IProgress prog = null)
    {
        base.ModifyRequest(request, prog);
        request.Header("Ocp-Apim-Subscription-Key", subscriptionKey)
            .Body(new StringContent(""), MediaType.Application_X_Www_Form_Urlencoded);
    }
}
