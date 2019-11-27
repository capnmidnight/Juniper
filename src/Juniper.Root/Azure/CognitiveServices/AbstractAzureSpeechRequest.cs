using System;
using System.Net;

using Juniper.HTTP;

namespace Juniper.Azure.CognitiveServices
{
    public abstract class AbstractAzureSpeechRequest<MediaTypeT> : AbstractAzureRequest<MediaTypeT>
        where MediaTypeT : MediaType
    {
        protected AbstractAzureSpeechRequest(HttpMethod method, string region, string path, MediaTypeT contentType)
            : base(method, region, "tts.speech", path, contentType)
        { }

        public string AuthToken { get; set; }

        protected override void ModifyRequest(HttpWebRequest request)
        {
            if(string.IsNullOrEmpty(AuthToken))
            {
                throw new InvalidOperationException("An AuthToken is required to be able to submit this request.");
            }
            base.ModifyRequest(request);
            request.Header("Authorization", $"Bearer {AuthToken}");
        }
    }
}
