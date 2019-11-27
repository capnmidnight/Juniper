using Juniper.HTTP;

namespace Juniper.Azure.CognitiveServices
{
    public class VoiceListRequest : AbstractAzureSpeechRequest<MediaType.Application>
    {
        public VoiceListRequest(string region)
            : base(HttpMethod.GET, region, "cognitiveservices/voices/list", Juniper.MediaType.Application.Json)
        { }
    }
}
