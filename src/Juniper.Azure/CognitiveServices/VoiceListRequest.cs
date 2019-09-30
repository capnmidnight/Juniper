using Juniper.HTTP;

namespace Juniper.Azure.CognitiveServices
{
    public class VoiceListRequest : AbstractAzureSpeechRequest
    {
        public VoiceListRequest(string region, string authToken)
            : base(region, "cognitiveservices/voices/list", authToken, MediaType.Application.Json)
        { }

        public VoiceListRequest(string region)
            : this(region, null)
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
