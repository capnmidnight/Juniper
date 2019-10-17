namespace Juniper.Azure.CognitiveServices
{
    public class VoiceListRequest : AbstractAzureSpeechRequest<MediaType.Application>
    {
        public VoiceListRequest(string region)
            : base(region, "cognitiveservices/voices/list", Juniper.MediaType.Application.Json)
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
