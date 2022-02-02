using Juniper.HTTP;

using System.Net.Http;

namespace Juniper.Speech.Azure.CognitiveServices
{
    public class VoiceListRequest : AbstractAzureSpeechRequest<MediaType.Application>
    {
        public VoiceListRequest(string region)
            : base(HttpMethod.Get, region, "cognitiveservices/voices/list", Juniper.MediaType.Application.Json, false)
        { }

        protected override string InternalCacheID => "voices";
    }
}
