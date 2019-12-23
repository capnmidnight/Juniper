using Juniper.HTTP;

namespace Juniper.Azure.CognitiveServices
{
    public class VoiceListRequest : AbstractAzureSpeechRequest<MediaType.Application>
    {
        public VoiceListRequest(string region)
            : base(HttpMethods.GET, region, "cognitiveservices/voices/list", Juniper.MediaType.Application.Json)
        { }
    }
}
