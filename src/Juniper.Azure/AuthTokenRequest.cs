using System.Net;
using System.Threading.Tasks;

using Juniper.HTTP;

namespace Juniper.Azure
{
    public class AuthTokenRequest : AbstractAzureRequest
    {
        private readonly string subscriptionKey;

        public AuthTokenRequest(string region, string subscriptionKey)
            : base(region, "api.cognitive", "sts/v1.0/issueToken", MediaType.Text.Plain)
        {
            this.subscriptionKey = subscriptionKey;
        }

        protected override async Task ModifyRequest(HttpWebRequest request)
        {
            await base.ModifyRequest(request);
            request.Header("Ocp-Apim-Subscription-Key", subscriptionKey);
        }

        protected override BodyInfo GetBodyInfo()
        {
            return new BodyInfo(MediaType.Application.X_Www_Form_Urlencoded, 0);
        }

        protected override ActionDelegate Action
        {
            get
            {
                return Post;
            }
        }
    }
}
