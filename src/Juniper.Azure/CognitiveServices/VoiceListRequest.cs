using Juniper.HTTP;

namespace Juniper.Azure.CognitiveServices
{
    public class VoiceListRequest : AbstractAzureSpeechRequest<MediaType.Application>
    {
        public VoiceListRequest(string region)
            : base(region, "cognitiveservices/voices/list", MediaType.Application.Json)
        { }

        protected override ActionDelegate Action
        {
            get
            {
                return Get;
            }
        }
    }
}
