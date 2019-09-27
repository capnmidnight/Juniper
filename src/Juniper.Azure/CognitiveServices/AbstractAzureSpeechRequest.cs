using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.Azure.CognitiveServices
{
    public abstract class AbstractAzureSpeechRequest : AbstractAzureRequest
    {
        private readonly string authToken;

        protected AbstractAzureSpeechRequest(string region, string path, string authToken, DirectoryInfo cacheLocation)
            : base(region, "tts.speech", path, cacheLocation)
        {
            this.authToken = authToken;
        }

        protected override async Task ModifyRequest(HttpWebRequest request)
        {
            await base.ModifyRequest(request);
            request.Header("Authorization", $"Bearer {authToken}");
        }
    }
}
