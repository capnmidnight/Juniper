using Juniper.HTTP;
using Juniper.Progress;

using System.Net;
using System.Net.Http;

namespace Juniper.Speech.Azure
{
    public class AuthTokenRequest : AbstractAzureRequest<MediaType.Text>
    {
        private readonly string subscriptionKey;

        public AuthTokenRequest(string region, string subscriptionKey)
            : base(HttpMethod.Post, region, "api.cognitive", "sts/v1.0/issueToken", MediaType.Text.Plain)
        {
            this.subscriptionKey = subscriptionKey;
        }

        protected override string InternalCacheID => null;

        protected override void ModifyRequest(HttpRequestMessage request, IProgress prog = null)
        {
            base.ModifyRequest(request);
            request.Header("Ocp-Apim-Subscription-Key", subscriptionKey)
                .ContentType(MediaType.Application.X_Www_Form_Urlencoded);
        }
    }
}
