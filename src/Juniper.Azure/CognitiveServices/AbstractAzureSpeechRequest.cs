using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.Azure.CognitiveServices
{
    public abstract class AbstractAzureSpeechRequest : AbstractAzureRequest
    {
        public string AuthToken { get; set; }

        protected AbstractAzureSpeechRequest(string region, string path, string authToken, DirectoryInfo cacheLocation)
            : base(region, "tts.speech", path, cacheLocation)
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
