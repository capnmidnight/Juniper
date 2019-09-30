using System;
using System.Net;
using System.Threading.Tasks;

using Juniper.HTTP;

namespace Juniper.Azure.CognitiveServices
{
    public abstract class AbstractAzureSpeechRequest : AbstractAzureRequest
    {
        public string AuthToken { get; set; }

        protected AbstractAzureSpeechRequest(string region, string path, string authToken, MediaType contentType)
            : base(region, "tts.speech", path, contentType)
        {
            AuthToken = authToken;
        }

        protected override async Task ModifyRequest(HttpWebRequest request)
        {
            if(string.IsNullOrEmpty(AuthToken))
            {
                throw new InvalidOperationException("An AuthToken is required to be able to submit this request.");
            }
            await base.ModifyRequest(request);
            request.Header("Authorization", $"Bearer {AuthToken}");
        }
    }
}
