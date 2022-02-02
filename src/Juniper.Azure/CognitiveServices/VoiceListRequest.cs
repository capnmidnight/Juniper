using System.Net.Http;

namespace Juniper.Speech.Azure.CognitiveServices
{
    public class VoiceListRequest : AbstractAzureSpeechRequest<MediaType.Application>
    {
        public VoiceListRequest(string region)
            : base(HttpMethod.Get, region, "cognitiveservices/voices/list", MediaType.Application.Json)
        { }

        protected override string InternalCacheID => "voices";
    }
}
