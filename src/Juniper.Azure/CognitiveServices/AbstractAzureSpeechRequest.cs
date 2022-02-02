
using System;
using System.Net.Http;

namespace Juniper.Speech.Azure.CognitiveServices
{
    public abstract class AbstractAzureSpeechRequest<MediaTypeT> : AbstractAzureRequest<MediaTypeT>
        where MediaTypeT : MediaType
    {
        protected AbstractAzureSpeechRequest(HttpMethod method, string region, string path, MediaTypeT contentType, bool hasRequestBody)
            : base(method, region, "tts.speech", path, contentType, hasRequestBody)
        { }

        public string AuthToken { get; set; }

        protected override void ModifyRequest(HttpRequestMessage request)
        {
            if (string.IsNullOrEmpty(AuthToken))
            {
                throw new InvalidOperationException("An AuthToken is required to be able to submit this request.");
            }

            base.ModifyRequest(request);
            request.Header("Authorization", $"Bearer {AuthToken}");
        }
    }
}
